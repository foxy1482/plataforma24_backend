using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de tareas.</summary>
    public class TareaService : ITareaService
    {
        private readonly ITareaRepository _tareas;
        private readonly IUnitOfWork _unitOfWork;

        public TareaService(ITareaRepository tareas, IUnitOfWork unitOfWork)
        {
            _tareas = tareas;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TareaDTO>> ListarAsync(int? idMateria)
        {
            var tareas = await _tareas.ListarAsync(idMateria);
            return tareas.Select(t => t.ToDTO());
        }

        /// <inheritdoc />
        public async Task<TareaDTO> BuscarAsync(int id)
        {
            var tarea = await _tareas.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la tarea {id}.");

            return tarea.ToDTO();
        }

        /// <inheritdoc />
        public async Task<TareaDTO> CrearAsync(CrearTareaDTO dto)
        {
            ValidarFechas(dto.FechaEntrega, dto.PublicadaDesde);

            var tarea = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaEntrega = dto.FechaEntrega,
                PublicadaDesde = dto.PublicadaDesde,
                IdMateria = dto.IdMateria
            };

            await _tareas.AddAsync(tarea);
            await _unitOfWork.SaveChangesAsync();

            return tarea.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarTareaDTO dto)
        {
            var tarea = await _tareas.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la tarea {id}.");

            if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
            if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
            if (dto.FechaEntrega.HasValue) tarea.FechaEntrega = dto.FechaEntrega.Value;

            ValidarFechas(tarea.FechaEntrega, tarea.PublicadaDesde);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var tarea = await _tareas.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró la tarea {id}.");

            _tareas.Remove(tarea);
            await _unitOfWork.SaveChangesAsync();
        }

        private static void ValidarFechas(DateTime fechaEntrega, DateTime publicadaDesde)
        {
            if (fechaEntrega < publicadaDesde)
                throw new BadRequestException("La fecha de entrega no puede ser anterior a la fecha de publicación.");
        }
    }
}
