using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Archivos asociados a las tareas.</summary>
    [ApiController]
    [Route("tareas/archivos")]
    [Authorize]
    public class TareaArchivosController : ControllerBase
    {
        private readonly ITareaArchivoService _service;

        public TareaArchivosController(ITareaArchivoService service)
        {
            _service = service;
        }

        /// <summary>Lista los archivos de una tarea.</summary>
        /// <param name="idTarea">Id de la tarea.</param>
        // GET /tareas/archivos?idTarea={id}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaArchivoDTO>>> ListarArchivosTarea([FromQuery] int idTarea)
        {
            return Ok(await _service.ListarAsync(idTarea));
        }

        /// <summary>Asocia un archivo a una tarea. Administradores y profesores.</summary>
        /// <param name="dto">Tarea y archivo.</param>
        // POST /tareas/archivos
        [HttpPost]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> VincularArchivo([FromBody] VincTareaArchivoDTO dto)
        {
            await _service.VincularAsync(dto);
            return NoContent();
        }

        /// <summary>Quita un archivo de una tarea. Administradores y profesores.</summary>
        /// <param name="idTarea">Id de la tarea.</param>
        /// <param name="idArchivo">Id del archivo.</param>
        // DELETE /tareas/archivos?idTarea={id}&idArchivo={id}
        [HttpDelete]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idTarea, [FromQuery] int idArchivo)
        {
            await _service.DesvincularAsync(idTarea, idArchivo);
            return NoContent();
        }
    }
}
