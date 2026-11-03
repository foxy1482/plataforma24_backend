using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de tareas.</summary>
    public class TareaRepository : Repository<Tarea>, ITareaRepository
    {
        public TareaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<Tarea>> ListarAsync(int? idMateria)
        {
            var consulta = DbSet.AsNoTracking();

            if (idMateria.HasValue && idMateria != 0)
            {
                consulta = consulta.Where(t => t.IdMateria == idMateria);
            }

            return await consulta.ToListAsync();
        }
    }
}
