using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Entregas;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Entregas de tareas realizadas por los alumnos.</summary>
    [ApiController]
    [Route("entregas")]
    [Authorize]
    public class EntregasController : ControllerBase
    {
        private readonly IEntregaService _service;

        public EntregasController(IEntregaService service)
        {
            _service = service;
        }

        /// <summary>Lista las entregas de un alumno. Un alumno solo consulta las propias.</summary>
        /// <param name="idUsuario">Id del alumno.</param>
        // GET /entregas/alumno/{idUsuario}
        [HttpGet("alumno/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<EntregaRespuestaDTO>>> BuscarPorAlumno(int idUsuario)
        {
            return Ok(await _service.ListarPorAlumnoAsync(idUsuario));
        }

        /// <summary>Lista las entregas de una tarea. Solo administradores.</summary>
        /// <param name="idTarea">Id de la tarea.</param>
        // GET /entregas/tarea/{idTarea}
        [HttpGet("tarea/{idTarea}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<IEnumerable<EntregaRespuestaDTO>>> ListarPorTarea(int idTarea)
        {
            return Ok(await _service.ListarPorTareaAsync(idTarea));
        }

        /// <summary>Lista las entregas de un alumno para las tareas de una materia.</summary>
        /// <param name="idMateria">Id de la materia.</param>
        /// <param name="idUsuario">Id del alumno.</param>
        // GET /materias/{idMateria}/entregas/alumno/{idUsuario}
        [HttpGet("/materias/{idMateria}/entregas/alumno/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<EntregaRespuestaDTO>>> ListarPorMateriaYAlumno(int idMateria, int idUsuario)
        {
            return Ok(await _service.ListarPorMateriaYAlumnoAsync(idMateria, idUsuario));
        }

        /// <summary>Registra la entrega del usuario autenticado para una tarea.</summary>
        /// <param name="dto">Tarea entregada.</param>
        /// <response code="201">Entrega registrada.</response>
        /// <response code="400">La tarea no existe o ya fue entregada.</response>
        // POST /entregas
        [HttpPost]
        public async Task<ActionResult<EntregaRespuestaDTO>> RealizarEntrega([FromBody] EntregaDTO dto)
        {
            var creada = await _service.RealizarAsync(dto);
            return CreatedAtAction(nameof(BuscarPorAlumno), new { idUsuario = creada.IdUsuario }, creada);
        }

        /// <summary>Modifica una entrega. Solo un administrador puede asignar la calificacion.</summary>
        /// <param name="id">Id de la entrega.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /entregas/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> ModificarEntrega(int id, [FromBody] ModificarEntregaDTO dto)
        {
            await _service.ModificarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina una entrega propia (o cualquiera, si es administrador).</summary>
        /// <param name="id">Id de la entrega.</param>
        // DELETE /entregas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEntrega(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
