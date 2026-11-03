using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de los archivos de un material.</summary>
    public class MaterialArchivoRepository : Repository<MaterialArchivo>, IMaterialArchivoRepository
    {
        public MaterialArchivoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<Archivo>> ListarArchivosAsync(int idMaterial)
        {
            return (from v in DbSet.AsNoTracking()
                    where v.IdMaterial == idMaterial
                    join a in Context.Archivos on v.IdArchivo equals a.Id
                    select a).ToListAsync();
        }

        /// <inheritdoc />
        public Task<MaterialArchivo?> GetVinculoAsync(int idMaterial, int idArchivo)
        {
            return DbSet.FirstOrDefaultAsync(v => v.IdMaterial == idMaterial && v.IdArchivo == idArchivo);
        }

        /// <inheritdoc />
        public Task<bool> ExisteVinculoAsync(int idMaterial, int idArchivo)
        {
            return DbSet.AnyAsync(v => v.IdMaterial == idMaterial && v.IdArchivo == idArchivo);
        }
    }
}
