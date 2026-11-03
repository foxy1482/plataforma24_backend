using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Periodos;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Periodos bimestrales de cada ciclo lectivo.</summary>
    [ApiController]
    [Route("periodos")]
    [Authorize]
    public class PeriodosController : ControllerBase
    {
        private readonly IPeriodoService _service;

        public PeriodosController(IPeriodoService service)
        {
            _service = service;
        }

        /// <summary>Lista los periodos, opcionalmente de un ciclo lectivo.</summary>
        /// <param name="ciclo">Ciclo lectivo.</param>
        // GET /periodos?ciclo={anio}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoDTO>>> ListarPeriodos([FromQuery] int? ciclo)
        {
            return Ok(await _service.ListarAsync(ciclo));
        }

        /// <summary>Devuelve un periodo.</summary>
        /// <param name="id">Id del periodo.</param>
        /// <response code="404">El periodo no existe.</response>
        // GET /periodos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodoDTO>> BuscarPeriodo(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Crea un periodo. Solo administradores.</summary>
        /// <param name="dto">Ciclo, bimestre (1 a 4) y fechas.</param>
        /// <response code="201">Periodo creado.</response>
        /// <response code="400">Bimestre invalido, fechas incoherentes o periodo repetido.</response>
        // POST /periodos
        [HttpPost]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<PeriodoDTO>> CrearPeriodo([FromBody] PeriodoDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarPeriodo), new { id = creado.Id }, creado);
        }

        /// <summary>Cambia las fechas de un periodo. Solo administradores.</summary>
        /// <param name="id">Id del periodo.</param>
        /// <param name="dto">Nuevas fechas.</param>
        // PATCH /periodos/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> ActualizarPeriodo(int id, [FromBody] ModificarPeriodoDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un periodo. Solo administradores.</summary>
        /// <param name="id">Id del periodo.</param>
        // DELETE /periodos/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarPeriodo(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
