using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Boletines;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>GeneraciA³n, carga de notas y consulta de boletines. Un alumno solo ve sus boletines publicados.</summary>
    public class BoletinService : IBoletinService
    {
        private readonly IBoletinRepository _boletines;
        private readonly IBoletinNotaRepository _notas;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public BoletinService(
            IBoletinRepository boletines,
            IBoletinNotaRepository notas,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _boletines = boletines;
            _notas = notas;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BoletinResumenDTO>> ListarPorAlumnoAsync(int idAlumno, int? ciclo)
        {
            bool esPersonal = _currentUser.EsPersonal;

            if (!esPersonal && _currentUser.Id != idAlumno) throw new ForbiddenException();

            var resumenes = await _boletines.ListarResumenPorAlumnoAsync(idAlumno, ciclo, soloPublicados: !esPersonal);
            return resumenes.Select(r => r.ToDTO());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BoletinResumenDTO>> ListarPorCursoAsync(int idCurso, int? ciclo, short? bimestre)
        {
            var resumenes = await _boletines.ListarResumenPorCursoAsync(idCurso, ciclo, bimestre);
            return resumenes.Select(r => r.ToDTO());
        }

        /// <inheritdoc />
        public async Task<BoletinDetalleDTO> BuscarAsync(int id)
        {
            var resumen = await _boletines.GetResumenAsync(id)
                ?? throw new NotFoundException($"No se encontró el boletín {id}.");

            if (!_currentUser.EsPersonal)
            {
                if (resumen.IdAlumno != _currentUser.Id) throw new ForbiddenException();

                if (resumen.Estado != EstadoBoletin.publicado)
                    throw new NotFoundException($"No se encontró el boletín {id}.");
            }

            var notas = await _boletines.ListarDetalleAsync(id);

            return new BoletinDetalleDTO { Resumen = resumen.ToDTO(), Notas = notas.Select(n => n.ToDTO()).ToList() };
        }

        /// <inheritdoc />
        public Task<int> GenerarAsync(GenerarBoletinDTO dto)
        {
            return _boletines.GenerarAsync(dto.IdAlumno, dto.IdPeriodo, _currentUser.Id);
        }

        /// <inheritdoc />
        public Task<int> GenerarPorCursoAsync(GenerarBoletinesCursoDTO dto)
        {
            return _boletines.GenerarPorCursoAsync(dto.IdCurso, dto.IdPeriodo, _currentUser.Id);
        }

        /// <inheritdoc />
        public async Task CargarNotaAsync(int idBoletin, int idMateria, ModificarNotaDTO dto)
        {
            var nota = await _notas.GetAsync(idBoletin, idMateria)
                ?? throw new NotFoundException("Ese boletín no tiene esa materia.");

            if (dto.Nota.HasValue) nota.Nota = dto.Nota.Value;
            if (dto.Observacion != null) nota.Observacion = dto.Observacion;

            nota.IdCargadoPor = _currentUser.Id;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarBoletinDTO dto)
        {
            var boletin = await _boletines.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el boletín {id}.");

            if (dto.Observaciones != null) boletin.Observaciones = dto.Observaciones;

            if (dto.Estado.HasValue) boletin.Estado = dto.Estado.Value;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var boletin = await _boletines.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el boletín {id}.");

            _boletines.Remove(boletin);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
