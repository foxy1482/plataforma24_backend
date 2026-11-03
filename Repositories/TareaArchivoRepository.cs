using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de los archivos de una tarea.</summary>
    public class TareaArchivoRepository : Repository<TareaArchivo>, ITareaArchivoRepository
    {
        public TareaArchivoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<Archivo>> ListarArchivosAsync(int idTarea)
        {
            return (from v in DbSet.AsNoTracking()
                    where v.IdTarea == idTarea
                    join a in Context.Archivos on v.IdArchivo equals a.Id
                    select a).ToListAsync();
        }

        /// <inheritdoc />
        public Task<TareaArchivo?> GetVinculoAsync(int idTarea, int idArchivo)
        {
            return DbSet.FirstOrDefaultAsync(v => v.IdTarea == idTarea && v.IdArchivo == idArchivo);
        }

        /// <inheritdoc />
        public Task<bool> ExisteVinculoAsync(int idTarea, int idArchivo)
        {
            return DbSet.AnyAsync(v => v.IdTarea == idTarea && v.IdArchivo == idArchivo);
        }
    }
}
