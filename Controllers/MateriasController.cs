using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Materias;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Consulta y administracion de materias.</summary>
    [ApiController]
    [Route("materias")]
    [Authorize]
    public class MateriasController : ControllerBase
    {
        private readonly IMateriaService _service;

        public MateriasController(IMateriaService service)
        {
            _service = service;
        }

        /// <summary>Lista las materias, con filtros opcionales.</summary>
        /// <param name="profesor">Id del profesor a cargo.</param>
        /// <param name="curso">Id del curso.</param>
        // GET /materias?profesor={idProfesor}&curso={idCurso}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaDTO>>> ListarMaterias(
            [FromQuery] int? profesor,
            [FromQuery] int? curso)
        {
            return Ok(await _service.ListarAsync(profesor, curso));
        }

        /// <summary>Devuelve una materia.</summary>
        /// <param name="id">Id de la materia.</param>
        /// <response code="404">La materia no existe.</response>
        // GET /materias/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaDTO>> BuscarMateria(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Crea una materia. El profesor queda a cargo de las que crea y elige el curso; el administrador asigna el profesor.</summary>
        /// <param name="dto">Datos de la materia.</param>
        /// <response code="201">Materia creada.</response>
        /// <response code="400">Datos invalidos o el curso ya tiene una materia con ese nombre.</response>
        /// <response code="403">Solo profesores y administradores crean materias.</response>
        // POST /materias
        [HttpPost]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<ActionResult<MateriaDTO>> CrearMateria([FromBody] MateriaDTO dto)
        {
            var creada = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarMateria), new { id = creada.Id }, creada);
        }

        /// <summary>Modifica una materia. El profesor, solo las suyas; el administrador, cualquiera.</summary>
        /// <param name="id">Id de la materia.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /materias/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> ActualizarMateria(int id, [FromBody] ModificarMateriaDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina una materia. El profesor, solo las suyas; el administrador, cualquiera.</summary>
        /// <param name="id">Id de la materia.</param>
        // DELETE /materias/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> EliminarMateria(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
