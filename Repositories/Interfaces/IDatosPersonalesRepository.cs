using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="DatosPersonales"/>.</summary>
    public interface IDatosPersonalesRepository : IRepository<DatosPersonales>
    {
        Task<DatosPersonales?> GetByUsuarioIdAsync(int idUsuario);

        Task<bool> ExistePorUsuarioAsync(int idUsuario);

        Task<bool> ExistePorDniAsync(long dni);

        /// <summary>Lista los datos personales de los usuarios asignados a un curso.</summary>
        Task<List<DatosPersonales>> ListarPorCursoAsync(int idCurso);
    }
}
