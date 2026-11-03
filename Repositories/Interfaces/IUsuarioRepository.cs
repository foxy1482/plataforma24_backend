using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Usuario"/>.</summary>
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);

        Task<bool> ExisteAsync(int id);

        Task<bool> ExistePorEmailAsync(string email);

        Task<bool> ExistePorNombreUsuarioAsync(string nombreUsuario);

        Task<List<Usuario>> ListarPorCursoAsync(int idCurso);
    }
}
