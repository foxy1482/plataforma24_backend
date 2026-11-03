using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de usuarios.</summary>
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<Usuario?> GetByEmailAsync(string email)
        {
            return DbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(int id)
        {
            return DbSet.AnyAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public Task<bool> ExistePorEmailAsync(string email)
        {
            return DbSet.AnyAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public Task<bool> ExistePorNombreUsuarioAsync(string nombreUsuario)
        {
            return DbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario);
        }

        /// <inheritdoc />
        public Task<List<Usuario>> ListarPorCursoAsync(int idCurso)
        {
            return DbSet.AsNoTracking().Where(u => u.IdCurso == idCurso).ToListAsync();
        }
    }
}
