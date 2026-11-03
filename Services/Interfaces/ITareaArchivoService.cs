using Plataforma24API.DTOs.Tareas;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Archivos asociados a las tareas.</summary>
    public interface ITareaArchivoService
    {
        Task<IEnumerable<TareaArchivoDTO>> ListarAsync(int idTarea);

        /// <exception cref="Common.Exceptions.BadRequestException">La tarea o el archivo no existen, o ya estaban asociados.</exception>
        Task VincularAsync(VincTareaArchivoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no esta asociado a la tarea.</exception>
        Task DesvincularAsync(int idTarea, int idArchivo);
    }
}
