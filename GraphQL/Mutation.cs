using HotChocolate.Authorization;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.GraphQL
{
    /// <summary>Operaciones de escritura GraphQL. Delegan en los mismos servicios que los controladores.</summary>
    public class Mutation
    {
        /// <summary>Publica una tarea. Administradores y profesores.</summary>
        /// <param name="servicio">Servicio que HotChocolate inyecta en el resolver.</param>
        /// <param name="entrada">Datos de la tarea.</param>
        [Authorize(Policy = Policies.AdminOProfesor)]
        public Task<TareaDTO> CrearTarea(ITareaService servicio, CrearTareaDTO entrada)
        {
            return servicio.CrearAsync(entrada);
        }

        /// <summary>Elimina una tarea. Administradores y profesores.</summary>
        /// <param name="servicio">Servicio que HotChocolate inyecta en el resolver.</param>
        /// <param name="id">Id de la tarea.</param>
        /// <returns><c>true</c> cuando la tarea fue eliminada.</returns>
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<bool> EliminarTarea(ITareaService servicio, int id)
        {
            await servicio.EliminarAsync(id);
            return true;
        }
    }
}
