using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de entregas.</summary>
    public class EntregaRepository : Repository<Entrega>, IEntregaRepository
    {
        public EntregaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<Entrega>> ListarPorAlumnoAsync(int idUsuario)
        {
            return DbSet.AsNoTracking().Where(e => e.IdUsuario == idUsuario).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<Entrega>> ListarPorTareaAsync(int idTarea)
        {
            return DbSet.AsNoTracking().Where(e => e.IdTarea == idTarea).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<Entrega>> ListarPorMateriaYAlumnoAsync(int idMateria, int idUsuario)
        {
            return (from e in DbSet.AsNoTracking()
                    join t in Context.Tareas on e.IdTarea equals t.Id
                    where t.IdMateria == idMateria && e.IdUsuario == idUsuario
                    select e).ToListAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(int idTarea, int idUsuario)
        {
            return DbSet.AnyAsync(e => e.IdTarea == idTarea && e.IdUsuario == idUsuario);
        }
    }
}
