using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de periodos bimestrales.</summary>
    public class PeriodoRepository : Repository<PeriodoBimestral>, IPeriodoRepository
    {
        public PeriodoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<PeriodoBimestral>> ListarAsync(int? ciclo)
        {
            var consulta = DbSet.AsNoTracking();

            if (ciclo.HasValue)
            {
                consulta = consulta.Where(p => p.CicloLectivo == ciclo.Value);
            }

            return await consulta
                .OrderBy(p => p.CicloLectivo)
                .ThenBy(p => p.Bimestre)
                .ToListAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(int cicloLectivo, short bimestre)
        {
            return DbSet.AnyAsync(p => p.CicloLectivo == cicloLectivo && p.Bimestre == bimestre);
        }
    }
}
