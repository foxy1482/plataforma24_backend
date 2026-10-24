using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Asistencias;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("asistencias")]
    [Authorize]
    public class AsistenciasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsistenciasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirPlanilla([FromBody] AbrirAsistenciaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            int idUsuario = User.GetUserId()!.Value;

            try
            {
                var idAsistencia = await _context.Database
                    .SqlQuery<int>($"SELECT public.fn_abrir_asistencia({dto.IdCurso}, {dto.Fecha}, {idUsuario}) AS \"Value\"")
                    .SingleAsync();

                return Ok(new { idAsistencia });
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }
        }

        [HttpGet("curso/{idCurso}")]
        public async Task<ActionResult<PlanillaAsistenciaDTO>> ObtenerPlanilla(
            int idCurso,
            [FromQuery] DateOnly? fecha)
        {
            if (!User.EsPersonal()) return Forbid();

            var fechaConsulta = fecha ?? DateOnly.FromDateTime(DateTime.Today);

            var planilla = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.IdCurso == idCurso && a.Fecha == fechaConsulta);
            if (planilla == null)
                return NotFound("No hay planilla abierta para ese curso y fecha. Usá POST /asistencias/abrir.");

            var detalle = await (from d in _context.AsistenciaDetalles
                                 where d.IdAsistencia == planilla.Id
                                 join u in _context.Usuarios on d.IdAlumno equals u.Id
                                 join dp in _context.DatosPersonales on u.Id equals dp.IdUsuario into dps
                                 from dp in dps.DefaultIfEmpty()
                                 orderby dp.Apellido, dp.Nombre
                                 select new AsistenciaDetalleDTO
                                 {
                                     Id = d.Id,
                                     IdAlumno = d.IdAlumno,
                                     NumeroLegajo = u.NumeroLegajo,
                                     Apellido = dp.Apellido,
                                     Nombre = dp.Nombre,
                                     Estado = d.Estado,
                                     Observacion = d.Observacion
                                 }).ToListAsync();

            return Ok(new PlanillaAsistenciaDTO
            {
                Id = planilla.Id,
                IdCurso = planilla.IdCurso,
                Fecha = planilla.Fecha,
                Cerrada = planilla.Cerrada,
                IdRegistradoPor = planilla.IdRegistradoPor,
                Observaciones = planilla.Observaciones,
                Detalle = detalle
            });
        }

        [HttpGet("curso/{idCurso}/planillas")]
        public async Task<ActionResult<IEnumerable<AsistenciaDTO>>> ListarPlanillas(
            int idCurso,
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta)
        {
            if (!User.EsPersonal()) return Forbid();

            var cons = _context.Asistencias.Where(a => a.IdCurso == idCurso);

            if (desde.HasValue) cons = cons.Where(a => a.Fecha >= desde.Value);
            if (hasta.HasValue) cons = cons.Where(a => a.Fecha <= hasta.Value);

            var resultados = await cons.OrderByDescending(a => a.Fecha).ToListAsync();

            return Ok(resultados.Select(a => new AsistenciaDTO
            {
                Id = a.Id,
                IdCurso = a.IdCurso,
                Fecha = a.Fecha,
                Cerrada = a.Cerrada,
                IdRegistradoPor = a.IdRegistradoPor,
                Observaciones = a.Observaciones
            }));
        }

        [HttpPatch("{idAsistencia}/detalle")]
        public async Task<IActionResult> MarcarAsistencia(
            int idAsistencia,
            [FromBody] List<ModificarAsistenciaDetalleDTO> cambios)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            var planilla = await _context.Asistencias.FindAsync(idAsistencia);
            if (planilla == null) return NotFound();

            if (cambios == null || cambios.Count == 0)
                return BadRequest("Enviá al menos un alumno para modificar.");

            var idsAlumnos = cambios.Select(c => c.IdAlumno).ToList();

            var detalles = await _context.AsistenciaDetalles
                .Where(d => d.IdAsistencia == idAsistencia && idsAlumnos.Contains(d.IdAlumno))
                .ToListAsync();

            foreach (var cambio in cambios)
            {
                var det = detalles.FirstOrDefault(d => d.IdAlumno == cambio.IdAlumno);
                if (det == null)
                    return BadRequest($"El alumno {cambio.IdAlumno} no figura en esta planilla.");

                if (cambio.Estado.HasValue) det.Estado = cambio.Estado.Value;
                if (cambio.Observacion != null) det.Observacion = cambio.Observacion;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {

                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }

        [HttpPatch("{idAsistencia}")]
        public async Task<IActionResult> ActualizarPlanilla(int idAsistencia, [FromBody] ModificarAsistenciaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            var planilla = await _context.Asistencias.FindAsync(idAsistencia);
            if (planilla == null) return NotFound();

            if (dto.Cerrada.HasValue)
            {
                if (planilla.Cerrada && !dto.Cerrada.Value && !User.EsAdmin())
                    return Forbid();

                planilla.Cerrada = dto.Cerrada.Value;
            }

            if (dto.Observaciones != null) planilla.Observaciones = dto.Observaciones;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }

        [HttpDelete("{idAsistencia}")]
        public async Task<IActionResult> EliminarPlanilla(int idAsistencia)
        {
            if (!User.EsAdmin()) return Forbid();

            var planilla = await _context.Asistencias.FindAsync(idAsistencia);
            if (planilla == null) return NotFound();

            _context.Asistencias.Remove(planilla); 
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("alumno/{idAlumno}/resumen")]
        public async Task<ActionResult<ResumenAsistenciaDTO>> ResumenAlumno(int idAlumno, [FromQuery] int? ciclo)
        {
            if (!User.EsPersonal() && User.GetUserId() != idAlumno) return Forbid();

            var anuales = _context.VInasistenciasAnuales.Where(a => a.IdAlumno == idAlumno);
            var porBimestre = _context.VInasistenciasPeriodo.Where(p => p.IdAlumno == idAlumno);

            if (ciclo.HasValue)
            {
                anuales = anuales.Where(a => a.CicloLectivo == ciclo.Value);
                porBimestre = porBimestre.Where(p => p.CicloLectivo == ciclo.Value);
            }

            return Ok(new ResumenAsistenciaDTO
            {
                Anuales = await anuales.OrderBy(a => a.CicloLectivo).ToListAsync(),
                PorBimestre = await porBimestre
                    .OrderBy(p => p.CicloLectivo)
                    .ThenBy(p => p.Bimestre)
                    .ToListAsync()
            });
        }
    }
}