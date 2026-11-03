using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Rol"/>.</summary>
    public interface IRolRepository : IRepository<Rol>
    {
        Task<bool> ExistePorNombreAsync(string nombre);
    }
}
