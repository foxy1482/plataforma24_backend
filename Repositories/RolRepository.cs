using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de roles.</summary>
    public class RolRepository : Repository<Rol>, IRolRepository
    {
        public RolRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<bool> ExistePorNombreAsync(string nombre)
        {
            return DbSet.AnyAsync(r => r.Nombre == nombre);
        }
    }
}
