using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Foro;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Temas, mensajes y reportes del foro. El personal modera; el resto solo ve los temas generales y los de su curso.</summary>
    public class ForoService : IForoService
    {
        private readonly IForoTemaRepository _temas;
        private readonly IForoMensajeRepository _mensajes;
        private readonly IForoReporteRepository _reportes;
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public ForoService(
            IForoTemaRepository temas,
            IForoMensajeRepository mensajes,
            IForoReporteRepository reportes,
            IUsuarioRepository usuarios,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _temas = temas;
            _mensajes = mensajes;
            _reportes = reportes;
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        private bool EsModerador => _currentUser.EsPersonal;

        private async Task<int?> ObtenerMiCursoAsync()
        {
            var usuario = await _usuarios.GetByIdAsync(_currentUser.Id);
            return usuario?.IdCurso;
        }

        private async Task<bool> PuedeVerTemaAsync(int? idCursoDelTema)
        {
            if (EsModerador || idCursoDelTema == null) return true;

            return await ObtenerMiCursoAsync() == idCursoDelTema;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ForoTemaDTO>> ListarTemasAsync(int? idCurso)
        {
            bool restringir = !EsModerador;
            int? miCurso = restringir ? await ObtenerMiCursoAsync() : null;

            var temas = await _temas.ListarVistaAsync(idCurso, restringir, miCurso);
            return temas.Select(t => t.ToDTO());
        }

        /// <inheritdoc />
        public async Task<int> CrearTemaAsync(CrearTemaDTO dto)
        {
            int idUsuario = _currentUser.Id;

            // Un alumno abre debates solo en su propio curso; los temas generales (sin curso) y los de
            // otros cursos son del personal.
            int? idCurso = dto.IdCurso;
            if (!EsModerador)
            {
                int? miCurso = await ObtenerMiCursoAsync();
                if (idCurso == null) idCurso = miCurso;
                else if (idCurso != miCurso)
                    throw new ForbiddenException("Solo podés abrir debates en tu propio curso.");
            }

            var tema = new ForoTema
            {
                IdCurso = idCurso,
                IdAutor = idUsuario,
                Titulo = dto.Titulo
            };

            await using var transaccion = await _unitOfWork.BeginTransactionAsync();

            await _temas.AddAsync(tema);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(dto.PrimerMensaje))
            {
                await _mensajes.AddAsync(new ForoMensaje
                {
                    IdTema = tema.Id,
                    IdAutor = idUsuario,
                    Contenido = dto.PrimerMensaje
                });
                await _unitOfWork.SaveChangesAsync();
            }

            await transaccion.CommitAsync();

            return tema.Id;
        }

        /// <inheritdoc />
        public async Task ModerarTemaAsync(int idTema, ModificarTemaDTO dto)
        {
            var tema = await _temas.GetByIdAsync(idTema);
            if (tema == null || tema.Eliminado)
                throw new NotFoundException($"No se encontró el tema {idTema}.");

            if (dto.Fijado.HasValue) tema.Fijado = dto.Fijado.Value;
            if (dto.Cerrado.HasValue) tema.Cerrado = dto.Cerrado.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarTemaAsync(int idTema)
        {
            var tema = await _temas.GetByIdAsync(idTema)
                ?? throw new NotFoundException($"No se encontró el tema {idTema}.");

            if (tema.Eliminado) return;

            int idUsuario = _currentUser.Id;
            if (tema.IdAutor != idUsuario && !EsModerador) throw new ForbiddenException();

            tema.Eliminado = true;
            tema.IdEliminadoPor = idUsuario;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ForoMensajeDTO>> ListarMensajesAsync(int idTema)
        {
            var tema = await _temas.GetByIdAsync(idTema);
            if (tema == null || (tema.Eliminado && !EsModerador))
                throw new NotFoundException($"No se encontró el tema {idTema}.");

            if (!await PuedeVerTemaAsync(tema.IdCurso)) throw new ForbiddenException();

            var mensajes = await _mensajes.ListarVistaPorTemaAsync(idTema);
            return mensajes.Select(m => m.ToDTO());
        }

        /// <inheritdoc />
        public async Task<int> CrearMensajeAsync(int idTema, CrearMensajeDTO dto)
        {
            var tema = await _temas.GetByIdAsync(idTema);
            if (tema == null || tema.Eliminado)
                throw new NotFoundException($"No se encontró el tema {idTema}.");

            if (!await PuedeVerTemaAsync(tema.IdCurso)) throw new ForbiddenException();

            if (tema.Cerrado)
                throw new ConflictException("El tema está cerrado y no admite mensajes nuevos.");

            if (dto.IdRespuestaA.HasValue)
            {
                var original = await _mensajes.GetByIdAsync(dto.IdRespuestaA.Value);
                if (original == null || original.IdTema != idTema)
                    throw new BadRequestException("El mensaje al que respondés no pertenece a este tema.");
            }

            var mensaje = new ForoMensaje
            {
                IdTema = idTema,
                IdAutor = _currentUser.Id,
                IdRespuestaA = dto.IdRespuestaA,
                Contenido = dto.Contenido
            };

            await _mensajes.AddAsync(mensaje);
            await _unitOfWork.SaveChangesAsync();

            return mensaje.Id;
        }

        /// <inheritdoc />
        public async Task EditarMensajeAsync(int idMensaje, ModificarMensajeDTO dto)
        {
            var mensaje = await _mensajes.GetByIdAsync(idMensaje);
            if (mensaje == null || mensaje.Eliminado)
                throw new NotFoundException($"No se encontró el mensaje {idMensaje}.");

            if (mensaje.IdAutor != _currentUser.Id) throw new ForbiddenException();

            mensaje.Contenido = dto.Contenido;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarMensajeAsync(int idMensaje)
        {
            var mensaje = await _mensajes.GetByIdAsync(idMensaje)
                ?? throw new NotFoundException($"No se encontró el mensaje {idMensaje}.");

            if (mensaje.Eliminado) return;

            int idUsuario = _currentUser.Id;
            if (mensaje.IdAutor != idUsuario && !EsModerador) throw new ForbiddenException();

            mensaje.Eliminado = true;
            mensaje.IdEliminadoPor = idUsuario;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<int> ReportarMensajeAsync(int idMensaje, CrearReporteDTO dto)
        {
            var mensaje = await _mensajes.GetByIdAsync(idMensaje)
                ?? throw new NotFoundException($"No se encontró el mensaje {idMensaje}.");

            var tema = await _temas.GetByIdAsync(mensaje.IdTema);
            if (tema == null || !await PuedeVerTemaAsync(tema.IdCurso))
                throw new NotFoundException($"No se encontró el mensaje {idMensaje}.");

            int idUsuario = _currentUser.Id;

            if (await _reportes.ExisteAsync(idMensaje, idUsuario))
                throw new ConflictException("Ya reportaste este mensaje.");

            var reporte = new ForoReporte
            {
                IdMensaje = idMensaje,
                IdReportante = idUsuario,
                Motivo = dto.Motivo
            };

            await _reportes.AddAsync(reporte);
            await _unitOfWork.SaveChangesAsync();

            return reporte.Id;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ReporteDTO>> ListarReportesAsync(bool soloPendientes)
        {
            var filas = await _reportes.ListarConMensajeAsync(soloPendientes);
            return filas.Select(f => f.Reporte.ToDTO(f.Mensaje));
        }

        /// <inheritdoc />
        public async Task ResolverReporteAsync(int idReporte)
        {
            var reporte = await _reportes.GetByIdAsync(idReporte)
                ?? throw new NotFoundException($"No se encontró el reporte {idReporte}.");

            reporte.Resuelto = true;
            reporte.IdResueltoPor = _currentUser.Id;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
