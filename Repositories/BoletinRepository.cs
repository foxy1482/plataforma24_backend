using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos de boletines. La generacion se delega en funciones de la base de datos.</summary>
    public class BoletinRepository : Repository<Boletin>, IBoletinRepository
    {
        public BoletinRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public Task<List<VBoletinResumen>> ListarResumenPorAlumnoAsync(int idAlumno, int? ciclo, bool soloPublicados)
        {
            var consulta = Context.VBoletinResumenes.Where(b => b.IdAlumno == idAlumno);

            if (ciclo.HasValue)
            {
                consulta = consulta.Where(b => b.CicloLectivo == ciclo.Value);
            }

            if (soloPublicados)
            {
                consulta = consulta.Where(b => b.Estado == EstadoBoletin.publicado);
            }

            return consulta.OrderBy(b => b.CicloLectivo).ThenBy(b => b.Bimestre).ToListAsync();
        }

        /// <inheritdoc />
        public Task<List<VBoletinResumen>> ListarResumenPorCursoAsync(int idCurso, int? ciclo, short? bimestre)
        {
            var consulta = Context.VBoletinResumenes.Where(b => b.IdCurso == idCurso);

            if (ciclo.HasValue) consulta = consulta.Where(b => b.CicloLectivo == ciclo.Value);
            if (bimestre.HasValue) consulta = consulta.Where(b => b.Bimestre == bimestre.Value);

            return consulta.OrderBy(b => b.Apellido).ThenBy(b => b.Nombre).ToListAsync();
        }

        /// <inheritdoc />
        public Task<VBoletinResumen?> GetResumenAsync(int idBoletin)
        {
            return Context.VBoletinResumenes.FirstOrDefaultAsync(b => b.IdBoletin == idBoletin);
        }

        /// <inheritdoc />
        public Task<List<VBoletinDetalle>> ListarDetalleAsync(int idBoletin)
        {
            return Context.VBoletinDetalles
                .Where(d => d.IdBoletin == idBoletin)
                .OrderBy(d => d.Materia)
                .ToListAsync();
        }

        /// <inheritdoc />
        public Task<int> GenerarAsync(int idAlumno, int idPeriodo, int idCreador)
        {
            return Context.Database
                .SqlQuery<int>($"SELECT public.fn_generar_boletin({idAlumno}, {idPeriodo}, {idCreador}) AS \"Value\"")
                .SingleAsync();
        }

        /// <inheritdoc />
        public Task<int> GenerarPorCursoAsync(int idCurso, int idPeriodo, int idCreador)
        {
            return Context.Database
                .SqlQuery<int>($"SELECT public.fn_generar_boletines_curso({idCurso}, {idPeriodo}, {idCreador}) AS \"Value\"")
                .SingleAsync();
        }
    }
}
