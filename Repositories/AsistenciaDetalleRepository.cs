using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Acceso a datos del detalle de asistencia por alumno.</summary>
    public class AsistenciaDetalleRepository : Repository<AsistenciaDetalle>, IAsistenciaDetalleRepository
    {
        public AsistenciaDetalleRepository(AppDbContext context) : base(context) { }

        /// <inheritdoc />
        public async Task<List<(AsistenciaDetalle Detalle, Usuario Alumno, DatosPersonales? Datos)>> ListarConAlumnoAsync(int idAsistencia)
        {
            var filas = await (from d in DbSet.AsNoTracking()
                               where d.IdAsistencia == idAsistencia
                               join u in Context.Usuarios on d.IdAlumno equals u.Id
                               join dp in Context.DatosPersonales on u.Id equals dp.IdUsuario into dps
                               from dp in dps.DefaultIfEmpty()
                               orderby dp.Apellido, dp.Nombre
                               select new { Detalle = d, Alumno = u, Datos = (DatosPersonales?)dp }).ToListAsync();

            return filas.Select(f => (f.Detalle, f.Alumno, (DatosPersonales?)f.Datos)).ToList();
        }

        /// <inheritdoc />
        public Task<List<AsistenciaDetalle>> ListarPorAlumnosAsync(int idAsistencia, IEnumerable<int> idsAlumnos)
        {
            var ids = idsAlumnos.ToList();

            return DbSet
                .Where(d => d.IdAsistencia == idAsistencia && ids.Contains(d.IdAlumno))
                .ToListAsync();
        }
    }
}
