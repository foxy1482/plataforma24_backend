using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Material"/>.</summary>
    public interface IMaterialRepository : IRepository<Material>
    {
        /// <param name="idMateria">Materia; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<List<Material>> ListarAsync(int? idMateria);
    }
}
