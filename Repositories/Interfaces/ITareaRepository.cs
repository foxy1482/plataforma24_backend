using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Tarea"/>.</summary>
    public interface ITareaRepository : IRepository<Tarea>
    {
        /// <param name="idMateria">Materia; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<List<Tarea>> ListarAsync(int? idMateria);
    }
}
