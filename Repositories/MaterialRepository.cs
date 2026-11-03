using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de materiales didacticos.</summary>
    public class MaterialRepository : Repository<Material>, IMaterialRepository
    {
        public MaterialRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<Material>> ListarAsync(int? idMateria)
        {
            var consulta = DbSet.AsNoTracking();

            if (idMateria.HasValue && idMateria != 0)
            {
                consulta = consulta.Where(m => m.IdMateria == idMateria);
            }

            return await consulta.ToListAsync();
        }
    }
}
