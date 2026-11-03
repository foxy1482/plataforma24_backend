using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Materiales;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Archivos asociados a los materiales didacticos.</summary>
    [ApiController]
    [Route("materiales/archivos")]
    [Authorize]
    public class MaterialArchivosController : ControllerBase
    {
        private readonly IMaterialArchivoService _service;

        public MaterialArchivosController(IMaterialArchivoService service)
        {
            _service = service;
        }

        /// <summary>Lista los archivos de un material.</summary>
        /// <param name="idMaterial">Id del material.</param>
        // GET /materiales/archivos?idMaterial={id}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialArchivoDTO>>> ListarArchivosMaterial([FromQuery] int idMaterial)
        {
            return Ok(await _service.ListarAsync(idMaterial));
        }

        /// <summary>Asocia un archivo a un material. Administradores y profesores.</summary>
        /// <param name="dto">Material y archivo.</param>
        // POST /materiales/archivos
        [HttpPost]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> VincularArchivo([FromBody] VincMaterialArchivoDTO dto)
        {
            await _service.VincularAsync(dto);
            return NoContent();
        }

        /// <summary>Quita un archivo de un material. Administradores y profesores.</summary>
        /// <param name="idMaterial">Id del material.</param>
        /// <param name="idArchivo">Id del archivo.</param>
        // DELETE /materiales/archivos?idMaterial={id}&idArchivo={id}
        [HttpDelete]
        [Authorize(Policy = Policies.AdminOProfesor)]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idMaterial, [FromQuery] int idArchivo)
        {
            await _service.DesvincularAsync(idMaterial, idArchivo);
            return NoContent();
        }
    }
}
