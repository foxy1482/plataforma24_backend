using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos personales.</summary>
    public class DatosPersonalesRepository : Repository<DatosPersonales>, IDatosPersonalesRepository
    {
        public DatosPersonalesRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<DatosPersonales?> GetByUsuarioIdAsync(int idUsuario)
        {
            return DbSet.FirstOrDefaultAsync(d => d.IdUsuario == idUsuario);
        }

        /// <inheritdoc />
        public Task<bool> ExistePorUsuarioAsync(int idUsuario)
        {
            return DbSet.AnyAsync(d => d.IdUsuario == idUsuario);
        }

        /// <inheritdoc />
        public Task<bool> ExistePorDniAsync(long dni)
        {
            return DbSet.AnyAsync(d => d.DNI == dni);
        }

        /// <inheritdoc />
        public Task<bool> ExistePorDniDeOtroAsync(long dni, int idUsuario)
        {
            return DbSet.AnyAsync(d => d.DNI == dni && d.IdUsuario != idUsuario);
        }

        /// <inheritdoc />
        public async Task<List<DatosPersonales>> ListarPorCursoAsync(int idCurso)
        {
            return await (from u in Context.Usuarios.AsNoTracking()
                          join dp in Context.DatosPersonales on u.Id equals dp.IdUsuario
                          where u.IdCurso == idCurso
                          select dp).ToListAsync();
        }
    }
}
