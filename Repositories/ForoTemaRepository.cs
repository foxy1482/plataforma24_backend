using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de temas del foro.</summary>
    public class ForoTemaRepository : Repository<ForoTema>, IForoTemaRepository
    {
        public ForoTemaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<VForoTema>> ListarVistaAsync(int? idCurso, bool soloCursoYGeneral, int? miCurso)
        {
            var consulta = Context.VForoTemas.AsQueryable();

            if (idCurso.HasValue)
            {
                consulta = consulta.Where(t => t.IdCurso == idCurso.Value);
            }

            if (soloCursoYGeneral)
            {
                consulta = consulta.Where(t => t.IdCurso == null || t.IdCurso == miCurso);
            }

            return await consulta
                .OrderByDescending(t => t.Fijado)
                .ThenByDescending(t => t.UltimoMensaje ?? t.FechaCreacion)
                .ToListAsync();
        }
    }
}
