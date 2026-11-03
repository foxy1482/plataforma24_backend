using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Foro;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Palabras bloqueadas en el foro y verificacion de textos.</summary>
    [ApiController]
    [Route("foro/palabras")]
    [Authorize]
    public class PalabrasProhibidasController : ControllerBase
    {
        private readonly IPalabraProhibidaService _service;

        public PalabrasProhibidasController(IPalabraProhibidaService service)
        {
            _service = service;
        }

        /// <summary>Lista las palabras bloqueadas. Administradores y preceptores.</summary>
        // GET /foro/palabras
        [HttpGet]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<ActionResult<IEnumerable<PalabraDTO>>> ListarPalabras()
        {
            return Ok(await _service.ListarAsync());
        }

        /// <summary>Bloquea una palabra. Administradores y preceptores.</summary>
        /// <param name="dto">Palabra y modo de coincidencia.</param>
        /// <response code="201">Palabra bloqueada.</response>
        /// <response code="409">La palabra ya esta registrada.</response>
        // POST /foro/palabras
        [HttpPost]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<ActionResult<PalabraDTO>> CrearPalabra([FromBody] CrearPalabraDTO dto)
        {
            var creada = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ListarPalabras), null, creada);
        }

        /// <summary>Activa, desactiva o cambia el modo de una palabra. Administradores y preceptores.</summary>
        /// <param name="id">Id de la palabra.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /foro/palabras/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> ActualizarPalabra(int id, [FromBody] ModificarPalabraDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina una palabra de la lista. Solo administradores.</summary>
        /// <param name="id">Id de la palabra.</param>
        // DELETE /foro/palabras/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarPalabra(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }

        /// <summary>Indica si un texto contiene una palabra bloqueada, sin publicarlo.</summary>
        /// <param name="dto">Texto a revisar.</param>
        // POST /foro/palabras/verificar
        [HttpPost("verificar")]
        public async Task<ActionResult<VerificacionTextoDTO>> VerificarTexto([FromBody] VerificarTextoDTO dto)
        {
            return Ok(await _service.VerificarAsync(dto));
        }
    }
}
