using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Consulta y administracion de tareas.</summary>
    [ApiController]
    [Route("tareas")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly ITareaService _service;

        public TareasController(ITareaService service)
        {
            _service = service;
        }

        /// <summary>Lista las tareas, opcionalmente de una materia.</summary>
        /// <param name="materia">Id de la materia.</param>
        // GET /tareas?materia={idMateria}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaDTO>>> ListarTareas([FromQuery] int? materia)
        {
            return Ok(await _service.ListarAsync(materia));
        }

        /// <summary>Devuelve una tarea.</summary>
        /// <param name="id">Id de la tarea.</param>
        /// <response code="404">La tarea no existe.</response>
        // GET /tareas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TareaDTO>> BuscarTarea(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Publica una tarea. Administradores y profesores.</summary>
        /// <param name="dto">Datos de la tarea.</param>
        /// <response code="201">Tarea creada.</response>
        /// <response code="400">La fecha de entrega es anterior a la de publicacion.</response>
        // POST /tareas
        [HttpPost]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<ActionResult<TareaDTO>> CrearTarea([FromBody] CrearTareaDTO dto)
        {
            var creada = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarTarea), new { id = creada.Id }, creada);
        }

        /// <summary>Modifica una tarea. Administradores y profesores.</summary>
        /// <param name="id">Id de la tarea.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /tareas/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> ActualizarTarea(int id, [FromBody] ModificarTareaDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina una tarea. Administradores y profesores.</summary>
        /// <param name="id">Id de la tarea.</param>
        // DELETE /tareas/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
