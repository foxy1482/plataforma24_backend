using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Cursos;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Models;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Consulta y administracion de cursos.</summary>
    [ApiController]
    [Route("cursos")]
    [Authorize]
    public class CursosController : ControllerBase
    {
        private readonly ICursoService _service;

        public CursosController(ICursoService service)
        {
            _service = service;
        }

        /// <summary>Lista los cursos, con filtros opcionales.</summary>
        /// <param name="ano">Anio de cursada.</param>
        /// <param name="turno">Turno.</param>
        // GET /cursos?ano={anio}&turno={turno}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoDTO>>> ListarCursos(
            [FromQuery] Year? ano,
            [FromQuery] Turno? turno)
        {
            return Ok(await _service.ListarAsync(ano, turno));
        }

        /// <summary>Devuelve un curso.</summary>
        /// <param name="id">Id del curso.</param>
        /// <response code="404">El curso no existe.</response>
        // GET /cursos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDTO>> BuscarCurso(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Crea un curso. Preceptores y autoridades.</summary>
        /// <param name="dto">Datos del curso.</param>
        /// <response code="201">Curso creado.</response>
        /// <response code="400">Ya existe un curso con esa combinacion.</response>
        // POST /cursos
        [HttpPost]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<ActionResult<CursoDTO>> CrearCurso([FromBody] CursoDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarCurso), new { id = creado.Id }, creado);
        }

        /// <summary>Modifica un curso. Preceptores y autoridades.</summary>
        /// <param name="id">Id del curso.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /cursos/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> ActualizarCurso(int id, [FromBody] ModificarCursoDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un curso. Solo autoridades, y solo si no tiene alumnos ni materias asignados.</summary>
        /// <param name="id">Id del curso.</param>
        // DELETE /cursos/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarCurso(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
