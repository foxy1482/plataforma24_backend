using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Entregas;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Archivos adjuntos a las entregas.</summary>
    [ApiController]
    [Route("entregas/archivos")]
    [Authorize]
    public class EntregaArchivosController : ControllerBase
    {
        private readonly IEntregaArchivoService _service;

        public EntregaArchivosController(IEntregaArchivoService service)
        {
            _service = service;
        }

        /// <summary>Lista los adjuntos de una entrega. La ven su autor, los profesores y los administradores.</summary>
        /// <param name="idEntrega">Id de la entrega.</param>
        // GET /entregas/archivos?idEntrega={id}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntregaArchivoDTO>>> ListarArchivosEntrega([FromQuery] int idEntrega)
        {
            return Ok(await _service.ListarAsync(idEntrega));
        }

        /// <summary>Adjunta un archivo propio a una entrega propia.</summary>
        /// <param name="dto">Entrega y archivo.</param>
        // POST /entregas/archivos
        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincEntregaArchivoDTO dto)
        {
            await _service.VincularAsync(dto);
            return NoContent();
        }

        /// <summary>Quita un adjunto de una entrega.</summary>
        /// <param name="idEntrega">Id de la entrega.</param>
        /// <param name="idArchivo">Id del archivo.</param>
        // DELETE /entregas/archivos?idEntrega={id}&idArchivo={id}
        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idEntrega, [FromQuery] int idArchivo)
        {
            await _service.DesvincularAsync(idEntrega, idArchivo);
            return NoContent();
        }

        /// <summary>Quita todos los adjuntos de una entrega.</summary>
        /// <param name="idEntrega">Id de la entrega.</param>
        // DELETE /entregas/archivos/{idEntrega}
        [HttpDelete("{idEntrega}")]
        public async Task<IActionResult> EliminarTodosLosArchivosDeEntrega(int idEntrega)
        {
            await _service.DesvincularTodosAsync(idEntrega);
            return NoContent();
        }
    }
}
