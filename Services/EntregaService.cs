using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Entregas;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Entregas de tareas. Un alumno solo puede ver y modificar las suyas.</summary>
    public class EntregaService : IEntregaService
    {
        private readonly IEntregaRepository _entregas;
        private readonly ITareaRepository _tareas;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public EntregaService(
            IEntregaRepository entregas,
            ITareaRepository tareas,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser)
        {
            _entregas = entregas;
            _tareas = tareas;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EntregaRespuestaDTO>> ListarPorAlumnoAsync(int idUsuario)
        {
            if (!_currentUser.TienePermiso(idUsuario)) throw new ForbiddenException();

            var entregas = await _entregas.ListarPorAlumnoAsync(idUsuario);
            return entregas.Select(e => e.ToDTO());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EntregaRespuestaDTO>> ListarPorTareaAsync(int idTarea)
        {
            var entregas = await _entregas.ListarPorTareaAsync(idTarea);
            return entregas.Select(e => e.ToDTO());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EntregaRespuestaDTO>> ListarPorMateriaYAlumnoAsync(int idMateria, int idUsuario)
        {
            if (!_currentUser.TienePermiso(idUsuario)) throw new ForbiddenException();

            var entregas = await _entregas.ListarPorMateriaYAlumnoAsync(idMateria, idUsuario);
            return entregas.Select(e => e.ToDTO());
        }

        /// <inheritdoc />
        public async Task<EntregaRespuestaDTO> RealizarAsync(EntregaDTO dto)
        {
            int idUsuario = _currentUser.Id;

            if (await _tareas.GetByIdAsync(dto.IdTarea) == null)
                throw new BadRequestException("La tarea especificada no existe.");

            if (await _entregas.ExisteAsync(dto.IdTarea, idUsuario))
                throw new BadRequestException("Ya realizaste una entrega para esta tarea.");

            var entrega = new Entrega
            {
                IdTarea = dto.IdTarea,
                IdUsuario = idUsuario,
                FechaEntrega = DateTime.UtcNow
            };

            await _entregas.AddAsync(entrega);
            await _unitOfWork.SaveChangesAsync();

            return entrega.ToDTO();
        }

        /// <inheritdoc />
        public async Task ModificarAsync(int id, ModificarEntregaDTO dto)
        {
            var entrega = await _entregas.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la entrega {id}.");

            if (!_currentUser.TienePermiso(entrega.IdUsuario)) throw new ForbiddenException();

            if (dto.FechaEntrega.HasValue)
                entrega.FechaEntrega = dto.FechaEntrega.Value;

            if (dto.Calificacion != null && _currentUser.EsAdmin)
                entrega.Calificacion = dto.Calificacion;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var entrega = await _entregas.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la entrega {id}.");

            if (!_currentUser.TienePermiso(entrega.IdUsuario)) throw new ForbiddenException();

            _entregas.Remove(entrega);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
