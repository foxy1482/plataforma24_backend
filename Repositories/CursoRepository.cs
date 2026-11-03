using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de cursos.</summary>
    public class CursoRepository : Repository<Curso>, ICursoRepository
    {
        public CursoRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<Curso>> ListarAsync(Year? ano, Turno? turno)
        {
            var consulta = DbSet.AsNoTracking();

            if (ano.HasValue)
            {
                consulta = consulta.Where(c => c.Año == ano.Value);
            }

            if (turno.HasValue)
            {
                consulta = consulta.Where(c => c.Turno == turno.Value);
            }

            return await consulta.ToListAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(Year ano, Division division, Turno turno, int cicloLectivo)
        {
            return DbSet.AnyAsync(c =>
                c.Año == ano &&
                c.Division == division &&
                c.Turno == turno &&
                c.CicloLectivo == cicloLectivo);
        }
    }
}
