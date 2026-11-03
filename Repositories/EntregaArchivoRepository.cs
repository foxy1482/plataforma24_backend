using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de los archivos adjuntos a entregas.</summary>
    public class EntregaArchivoRepository : Repository<EntregaArchivo>, IEntregaArchivoRepository
    {
        public EntregaArchivoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<Archivo>> ListarArchivosAsync(int idEntrega)
        {
            return (from v in DbSet.AsNoTracking()
                    where v.IdEntrega == idEntrega
                    join a in Context.Archivos on v.IdArchivo equals a.Id
                    select a).ToListAsync();
        }

        /// <inheritdoc />
        public Task<EntregaArchivo?> GetVinculoAsync(int idEntrega, int idArchivo)
        {
            return DbSet.FirstOrDefaultAsync(v => v.IdEntrega == idEntrega && v.IdArchivo == idArchivo);
        }

        /// <inheritdoc />
        public Task<bool> ExisteVinculoAsync(int idEntrega, int idArchivo)
        {
            return DbSet.AnyAsync(v => v.IdEntrega == idEntrega && v.IdArchivo == idArchivo);
        }

        /// <inheritdoc />
        public Task<List<EntregaArchivo>> ListarVinculosAsync(int idEntrega)
        {
            return DbSet.Where(v => v.IdEntrega == idEntrega).ToListAsync();
        }

        /// <inheritdoc />
        public void RemoverVarios(IEnumerable<EntregaArchivo> vinculos)
        {
            DbSet.RemoveRange(vinculos);
        }
    }
}
