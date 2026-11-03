using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de reportes del foro.</summary>
    public class ForoReporteRepository : Repository<ForoReporte>, IForoReporteRepository
    {
        public ForoReporteRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<(ForoReporte Reporte, ForoMensaje Mensaje)>> ListarConMensajeAsync(bool soloPendientes)
        {
            var consulta = DbSet.AsNoTracking();

            if (soloPendientes)
            {
                consulta = consulta.Where(r => !r.Resuelto);
            }

            var filas = await (from r in consulta
                               join m in Context.ForoMensajes on r.IdMensaje equals m.Id
                               orderby r.Fecha descending
                               select new { Reporte = r, Mensaje = m }).ToListAsync();

            return filas.Select(f => (f.Reporte, f.Mensaje)).ToList();
        }

        /// <inheritdoc />
        public Task<bool> ExisteAsync(int idMensaje, int idReportante)
        {
            return DbSet.AnyAsync(r => r.IdMensaje == idMensaje && r.IdReportante == idReportante);
        }
    }
}
