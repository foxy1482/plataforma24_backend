using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de materias.</summary>
    public class MateriaRepository : Repository<Materia>, IMateriaRepository
    {
        public MateriaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<Materia>> ListarAsync(int? idProfesor, int? idCurso)
        {
            var consulta = DbSet.AsNoTracking();

            if (idProfesor.HasValue && idProfesor != 0)
            {
                consulta = consulta.Where(m => m.IdProfesor == idProfesor);
            }

            if (idCurso.HasValue && idCurso != 0)
            {
                consulta = consulta.Where(m => m.IdCurso == idCurso);
            }

            return await consulta.ToListAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(int idCurso, string nombre)
        {
            return DbSet.AnyAsync(m => m.IdCurso == idCurso && m.Nombre == nombre);
        }
    }
}
