using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de planillas de asistencia. La apertura de planillas se delega en la funcion <c>fn_abrir_asistencia</c> de la base de datos.</summary>
    public class AsistenciaRepository : Repository<Asistencia>, IAsistenciaRepository
    {
        public AsistenciaRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<int> AbrirPlanillaAsync(int idCurso, DateOnly fecha, int idUsuario)
        {
            return Context.Database
                .SqlQuery<int>($"SELECT public.fn_abrir_asistencia({idCurso}, {fecha}, {idUsuario}) AS \"Value\"")
                .SingleAsync();
        }

        /// <inheritdoc />
        public Task<Asistencia?> GetPorCursoYFechaAsync(int idCurso, DateOnly fecha)
        {
            return DbSet.FirstOrDefaultAsync(a => a.IdCurso == idCurso && a.Fecha == fecha);
        }

        /// <inheritdoc />
        public async Task<List<Asistencia>> ListarPorCursoAsync(int idCurso, DateOnly? desde, DateOnly? hasta)
        {
            var consulta = DbSet.AsNoTracking().Where(a => a.IdCurso == idCurso);

            if (desde.HasValue) consulta = consulta.Where(a => a.Fecha >= desde.Value);
            if (hasta.HasValue) consulta = consulta.Where(a => a.Fecha <= hasta.Value);

            return await consulta.OrderByDescending(a => a.Fecha).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<VInasistenciaAnual>> ListarInasistenciasAnualesAsync(int idAlumno, int? ciclo)
        {
            var consulta = Context.VInasistenciasAnuales.Where(a => a.IdAlumno == idAlumno);

            if (ciclo.HasValue) consulta = consulta.Where(a => a.CicloLectivo == ciclo.Value);

            return consulta.OrderBy(a => a.CicloLectivo).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<VInasistenciaPeriodo>> ListarInasistenciasPorBimestreAsync(int idAlumno, int? ciclo)
        {
            var consulta = Context.VInasistenciasPeriodo.Where(p => p.IdAlumno == idAlumno);

            if (ciclo.HasValue) consulta = consulta.Where(p => p.CicloLectivo == ciclo.Value);

            return consulta.OrderBy(p => p.CicloLectivo).ThenBy(p => p.Bimestre).ToListAsync();
        }
    }
}
