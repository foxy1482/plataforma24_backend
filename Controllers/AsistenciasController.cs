using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Asistencias;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Planillas de asistencia por curso y resumen de inasistencias por alumno.</summary>
    [ApiController]
    [Route("asistencias")]
    [Authorize]
    public class AsistenciasController : ControllerBase
    {
        private readonly IAsistenciaService _service;

        public AsistenciasController(IAsistenciaService service)
        {
            _service = service;
        }

        /// <summary>Abre la planilla de un curso para una fecha. Administradores y preceptores.</summary>
        /// <param name="dto">Curso y fecha.</param>
        /// <response code="200">Id de la planilla abierta.</response>
        // POST /asistencias/abrir
        [HttpPost("abrir")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> AbrirPlanilla([FromBody] AbrirAsistenciaDTO dto)
        {
            var idAsistencia = await _service.AbrirPlanillaAsync(dto);
            return Ok(new { idAsistencia });
        }

        /// <summary>Devuelve la planilla de un curso con el detalle de cada alumno. Solo personal.</summary>
        /// <param name="idCurso">Id del curso.</param>
        /// <param name="fecha">Dia consultado (aaaa-mm-dd); si se omite, hoy.</param>
        /// <response code="404">No hay planilla abierta para ese curso y fecha.</response>
        // GET /asistencias/curso/{idCurso}?fecha=2026-10-09
        [HttpGet("curso/{idCurso}")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<ActionResult<PlanillaAsistenciaDTO>> ObtenerPlanilla(int idCurso, [FromQuery] DateOnly? fecha)
        {
            return Ok(await _service.ObtenerPlanillaAsync(idCurso, fecha));
        }

        /// <summary>Lista las planillas de un curso en un rango de fechas. Solo personal.</summary>
        /// <param name="idCurso">Id del curso.</param>
        /// <param name="desde">Fecha inicial inclusive.</param>
        /// <param name="hasta">Fecha final inclusive.</param>
        // GET /asistencias/curso/{idCurso}/planillas?desde=2026-10-01&hasta=2026-10-31
        [HttpGet("curso/{idCurso}/planillas")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<ActionResult<IEnumerable<AsistenciaDTO>>> ListarPlanillas(
            int idCurso,
            [FromQuery] DateOnly? desde,
            [FromQuery] DateOnly? hasta)
        {
            return Ok(await _service.ListarPlanillasAsync(idCurso, desde, hasta));
        }

        /// <summary>Marca la asistencia de uno o mas alumnos. Administradores y preceptores.</summary>
        /// <param name="idAsistencia">Id de la planilla.</param>
        /// <param name="cambios">Estado u observacion por alumno.</param>
        /// <response code="409">La planilla esta cerrada.</response>
        // PATCH /asistencias/{idAsistencia}/detalle
        [HttpPatch("{idAsistencia}/detalle")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> MarcarAsistencia(
            int idAsistencia,
            [FromBody] List<ModificarAsistenciaDetalleDTO> cambios)
        {
            await _service.MarcarAsistenciaAsync(idAsistencia, cambios);
            return NoContent();
        }

        /// <summary>Cierra, reabre o anota una planilla. Administradores y preceptores.</summary>
        /// <param name="idAsistencia">Id de la planilla.</param>
        /// <param name="dto">Campos a modificar.</param>
        /// <response code="403">Solo administracion puede reabrir una planilla cerrada.</response>
        // PATCH /asistencias/{idAsistencia}
        [HttpPatch("{idAsistencia}")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> ActualizarPlanilla(int idAsistencia, [FromBody] ModificarAsistenciaDTO dto)
        {
            await _service.ActualizarPlanillaAsync(idAsistencia, dto);
            return NoContent();
        }

        /// <summary>Elimina una planilla. Solo administradores.</summary>
        /// <param name="idAsistencia">Id de la planilla.</param>
        // DELETE /asistencias/{idAsistencia}
        [HttpDelete("{idAsistencia}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarPlanilla(int idAsistencia)
        {
            await _service.EliminarPlanillaAsync(idAsistencia);
            return NoContent();
        }

        /// <summary>Inasistencias de un alumno, anuales y por bimestre. Un alumno solo consulta las propias.</summary>
        /// <param name="idAlumno">Id del alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; si se omite, todos.</param>
        // GET /asistencias/alumno/{idAlumno}/resumen?ciclo={anio}
        [HttpGet("alumno/{idAlumno}/resumen")]
        public async Task<ActionResult<ResumenAsistenciaDTO>> ResumenAlumno(int idAlumno, [FromQuery] int? ciclo)
        {
            return Ok(await _service.ResumenAlumnoAsync(idAlumno, ciclo));
        }
    }
}
