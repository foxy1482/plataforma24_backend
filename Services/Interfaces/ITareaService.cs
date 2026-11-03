using Plataforma24API.DTOs.Tareas;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de tareas.</summary>
    public interface ITareaService
    {
        /// <param name="idMateria">Materia; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<IEnumerable<TareaDTO>> ListarAsync(int? idMateria);

        /// <exception cref="Common.Exceptions.NotFoundException">La tarea no existe.</exception>
        Task<TareaDTO> BuscarAsync(int id);

        /// <exception cref="Common.Exceptions.BadRequestException">La fecha de entrega es anterior a la de publicacion.</exception>
        Task<TareaDTO> CrearAsync(CrearTareaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La tarea no existe.</exception>
        /// <exception cref="Common.Exceptions.BadRequestException">La fecha de entrega es anterior a la de publicacion.</exception>
        Task ActualizarAsync(int id, ModificarTareaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La tarea no existe.</exception>
        Task EliminarAsync(int id);
    }
}
