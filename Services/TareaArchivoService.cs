using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Archivos asociados a una tarea.</summary>
    public class TareaArchivoService : ITareaArchivoService
    {
        private readonly ITareaArchivoRepository _vinculos;
        private readonly ITareaRepository _tareas;
        private readonly IArchivoRepository _archivos;
        private readonly IUnitOfWork _unitOfWork;

        public TareaArchivoService(
            ITareaArchivoRepository vinculos,
            ITareaRepository tareas,
            IArchivoRepository archivos,
            IUnitOfWork unitOfWork)
        {
            _vinculos = vinculos;
            _tareas = tareas;
            _archivos = archivos;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TareaArchivoDTO>> ListarAsync(int idTarea)
        {
            var archivos = await _vinculos.ListarArchivosAsync(idTarea);
            return archivos.Select(a => a.ToTareaArchivoDTO(idTarea));
        }

        /// <inheritdoc />
        public async Task VincularAsync(VincTareaArchivoDTO dto)
        {
            var tarea = await _tareas.GetByIdAsync(dto.IdTarea);
            var archivo = await _archivos.GetByIdAsync(dto.IdArchivo);

            if (tarea == null || archivo == null)
                throw new BadRequestException("La tarea o el archivo especificado no existen.");

            if (await _vinculos.ExisteVinculoAsync(dto.IdTarea, dto.IdArchivo))
                throw new BadRequestException("Este archivo ya está vinculado a la tarea.");

            await _vinculos.AddAsync(new TareaArchivo
            {
                IdTarea = dto.IdTarea,
                IdArchivo = dto.IdArchivo
            });
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DesvincularAsync(int idTarea, int idArchivo)
        {
            var vinculo = await _vinculos.GetVinculoAsync(idTarea, idArchivo)
                ?? throw new NotFoundException("No se encontró el vínculo entre la tarea y el archivo.");

            _vinculos.Remove(vinculo);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
