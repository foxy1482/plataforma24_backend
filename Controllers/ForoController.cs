using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Foro;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Temas, mensajes y reportes del foro.</summary>
    [ApiController]
    [Route("foro")]
    [Authorize]
    public class ForoController : ControllerBase
    {
        private readonly IForoService _service;

        public ForoController(IForoService service)
        {
            _service = service;
        }

        /// <summary>Lista los temas visibles para el usuario: generales y de su curso (el personal ve todos).</summary>
        /// <param name="idCurso">Limita a los temas de ese curso.</param>
        // GET /foro/temas?idCurso={id}
        [HttpGet("temas")]
        public async Task<ActionResult<IEnumerable<ForoTemaDTO>>> ListarTemas([FromQuery] int? idCurso)
        {
            return Ok(await _service.ListarTemasAsync(idCurso));
        }

        /// <summary>Abre un tema nuevo, con un primer mensaje opcional.</summary>
        /// <param name="dto">Titulo, curso y primer mensaje.</param>
        /// <response code="201">Id del tema creado.</response>
        /// <response code="422">El texto contiene una palabra bloqueada.</response>
        // POST /foro/temas
        [HttpPost("temas")]
        public async Task<IActionResult> CrearTema([FromBody] CrearTemaDTO dto)
        {
            var id = await _service.CrearTemaAsync(dto);
            return CreatedAtAction(nameof(ListarMensajes), new { idTema = id }, new { id });
        }

        /// <summary>Fija o cierra un tema. Solo personal.</summary>
        /// <param name="idTema">Id del tema.</param>
        /// <param name="dto">Acciones de moderacion.</param>
        // PATCH /foro/temas/{idTema}
        [HttpPatch("temas/{idTema}")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<IActionResult> ModerarTema(int idTema, [FromBody] ModificarTemaDTO dto)
        {
            await _service.ModerarTemaAsync(idTema, dto);
            return NoContent();
        }

        /// <summary>Elimina un tema. Puede hacerlo su autor o el personal.</summary>
        /// <param name="idTema">Id del tema.</param>
        // DELETE /foro/temas/{idTema}
        [HttpDelete("temas/{idTema}")]
        public async Task<IActionResult> EliminarTema(int idTema)
        {
            await _service.EliminarTemaAsync(idTema);
            return NoContent();
        }

        /// <summary>Lista los mensajes de un tema en orden cronologico.</summary>
        /// <param name="idTema">Id del tema.</param>
        // GET /foro/temas/{idTema}/mensajes
        [HttpGet("temas/{idTema}/mensajes")]
        public async Task<ActionResult<IEnumerable<ForoMensajeDTO>>> ListarMensajes(int idTema)
        {
            return Ok(await _service.ListarMensajesAsync(idTema));
        }

        /// <summary>Publica un mensaje en un tema.</summary>
        /// <param name="idTema">Id del tema.</param>
        /// <param name="dto">Contenido y mensaje al que responde, si corresponde.</param>
        /// <response code="201">Id del mensaje creado.</response>
        /// <response code="409">El tema esta cerrado.</response>
        /// <response code="422">El texto contiene una palabra bloqueada.</response>
        // POST /foro/temas/{idTema}/mensajes
        [HttpPost("temas/{idTema}/mensajes")]
        public async Task<IActionResult> CrearMensaje(int idTema, [FromBody] CrearMensajeDTO dto)
        {
            var id = await _service.CrearMensajeAsync(idTema, dto);
            return CreatedAtAction(nameof(ListarMensajes), new { idTema }, new { id });
        }

        /// <summary>Edita un mensaje propio.</summary>
        /// <param name="idMensaje">Id del mensaje.</param>
        /// <param name="dto">Nuevo contenido.</param>
        // PATCH /foro/mensajes/{idMensaje}
        [HttpPatch("mensajes/{idMensaje}")]
        public async Task<IActionResult> EditarMensaje(int idMensaje, [FromBody] ModificarMensajeDTO dto)
        {
            await _service.EditarMensajeAsync(idMensaje, dto);
            return NoContent();
        }

        /// <summary>Elimina un mensaje. Puede hacerlo su autor o el personal.</summary>
        /// <param name="idMensaje">Id del mensaje.</param>
        // DELETE /foro/mensajes/{idMensaje}
        [HttpDelete("mensajes/{idMensaje}")]
        public async Task<IActionResult> EliminarMensaje(int idMensaje)
        {
            await _service.EliminarMensajeAsync(idMensaje);
            return NoContent();
        }

        /// <summary>Reporta un mensaje para que lo revise moderacion.</summary>
        /// <param name="idMensaje">Id del mensaje.</param>
        /// <param name="dto">Motivo del reporte.</param>
        /// <response code="200">Id del reporte creado.</response>
        /// <response code="409">El usuario ya reporto ese mensaje.</response>
        // POST /foro/mensajes/{idMensaje}/reportes
        [HttpPost("mensajes/{idMensaje}/reportes")]
        public async Task<IActionResult> ReportarMensaje(int idMensaje, [FromBody] CrearReporteDTO dto)
        {
            var id = await _service.ReportarMensajeAsync(idMensaje, dto);
            return Ok(new { id });
        }

        /// <summary>Lista los reportes con su mensaje. Solo personal.</summary>
        /// <param name="pendientes">Si es <c>false</c>, incluye tambien los resueltos.</param>
        // GET /foro/reportes?pendientes=false
        [HttpGet("reportes")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<ActionResult<IEnumerable<ReporteDTO>>> ListarReportes([FromQuery] bool pendientes = true)
        {
            return Ok(await _service.ListarReportesAsync(pendientes));
        }

        /// <summary>Marca un reporte como resuelto. Solo personal.</summary>
        /// <param name="idReporte">Id del reporte.</param>
        // PATCH /foro/reportes/{idReporte}/resolver
        [HttpPatch("reportes/{idReporte}/resolver")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<IActionResult> ResolverReporte(int idReporte)
        {
            await _service.ResolverReporteAsync(idReporte);
            return NoContent();
        }
    }
}
