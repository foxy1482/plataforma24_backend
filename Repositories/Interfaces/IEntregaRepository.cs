using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Entrega"/>.</summary>
    public interface IEntregaRepository : IRepository<Entrega>
    {
        Task<List<Entrega>> ListarPorAlumnoAsync(int idUsuario);

        Task<List<Entrega>> ListarPorTareaAsync(int idTarea);

        /// <summary>Lista las entregas de un alumno correspondientes a las tareas de una materia.</summary>
        Task<List<Entrega>> ListarPorMateriaYAlumnoAsync(int idMateria, int idUsuario);

        /// <summary>Indica si el alumno ya entrego la tarea.</summary>
        Task<bool> ExisteAsync(int idTarea, int idUsuario);
    }
}
