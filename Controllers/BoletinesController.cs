using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Boletines;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Generacion, carga de notas y consulta de boletines.</summary>
    [ApiController]
    [Route("boletines")]
    [Authorize]
    public class BoletinesController : ControllerBase
    {
        private readonly IBoletinService _service;

        public BoletinesController(IBoletinService service)
        {
            _service = service;
        }

        /// <summary>Lista los boletines de un alumno. Un alumno solo ve los suyos ya publicados.</summary>
        /// <param name="idAlumno">Id del alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; si se omite, todos.</param>
        // GET /boletines/alumno/{idAlumno}?ciclo={anio}
        [HttpGet("alumno/{idAlumno}")]
        public async Task<ActionResult<IEnumerable<BoletinResumenDTO>>> ListarPorAlumno(int idAlumno, [FromQuery] int? ciclo)
        {
            return Ok(await _service.ListarPorAlumnoAsync(idAlumno, ciclo));
        }

        /// <summary>Lista los boletines de un curso. Solo personal.</summary>
        /// <param name="idCurso">Id del curso.</param>
        /// <param name="ciclo">Ciclo lectivo; si se omite, todos.</param>
        /// <param name="bimestre">Bimestre de 1 a 4; si se omite, todos.</param>
        // GET /boletines/curso/{idCurso}?ciclo={anio}&bimestre={1-4}
        [HttpGet("curso/{idCurso}")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<ActionResult<IEnumerable<BoletinResumenDTO>>> ListarPorCurso(
            int idCurso,
            [FromQuery] int? ciclo,
            [FromQuery] short? bimestre)
        {
            return Ok(await _service.ListarPorCursoAsync(idCurso, ciclo, bimestre));
        }

        /// <summary>Devuelve un boletin con la nota de cada materia.</summary>
        /// <param name="id">Id del boletin.</param>
        /// <response code="403">Un alumno consulto el boletin de otro.</response>
        /// <response code="404">No existe o aun no fue publicado para el alumno.</response>
        // GET /boletines/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoletinDetalleDTO>> BuscarBoletin(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Genera el boletin de un alumno para un periodo. Administradores y preceptores.</summary>
        /// <param name="dto">Alumno y periodo.</param>
        /// <response code="200">Id del boletin generado.</response>
        // POST /boletines/generar
        [HttpPost("generar")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> GenerarBoletin([FromBody] GenerarBoletinDTO dto)
        {
            var idBoletin = await _service.GenerarAsync(dto);
            return Ok(new { idBoletin });
        }

        /// <summary>Genera los boletines de todo un curso. Administradores y preceptores.</summary>
        /// <param name="dto">Curso y periodo.</param>
        /// <response code="200">Cantidad de alumnos alcanzados.</response>
        // POST /boletines/generar-curso
        [HttpPost("generar-curso")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> GenerarBoletinesCurso([FromBody] GenerarBoletinesCursoDTO dto)
        {
            var alumnos = await _service.GenerarPorCursoAsync(dto);
            return Ok(new { alumnos });
        }

        /// <summary>Carga la nota u observacion de una materia. Solo personal.</summary>
        /// <param name="idBoletin">Id del boletin.</param>
        /// <param name="idMateria">Id de la materia.</param>
        /// <param name="dto">Nota (1 a 10) y/o observacion.</param>
        // PATCH /boletines/{idBoletin}/notas/{idMateria}
        [HttpPatch("{idBoletin}/notas/{idMateria}")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<IActionResult> CargarNota(int idBoletin, int idMateria, [FromBody] ModificarNotaDTO dto)
        {
            await _service.CargarNotaAsync(idBoletin, idMateria, dto);
            return NoContent();
        }

        /// <summary>Modifica las observaciones o publica un boletin. Administradores y preceptores.</summary>
        /// <param name="id">Id del boletin.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /boletines/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> ActualizarBoletin(int id, [FromBody] ModificarBoletinDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un boletin. Solo administradores.</summary>
        /// <param name="id">Id del boletin.</param>
        // DELETE /boletines/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarBoletin(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
