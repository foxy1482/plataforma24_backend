using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Archivos;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Subida, descarga y administracion de archivos.</summary>
    [ApiController]
    [Route("archivos")]
    [Authorize]
    public class ArchivosController : ControllerBase
    {
        private readonly IArchivoService _service;

        public ArchivosController(IArchivoService service)
        {
            _service = service;
        }

        /// <summary>Lista los archivos, con filtros opcionales.</summary>
        /// <param name="nombre">Nombre original exacto.</param>
        /// <param name="idUsuario">Propietario del archivo.</param>
        // GET /archivos?nombre={nombre}&idUsuario={idUsuario}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivoDTO>>> ListarArchivos(
            [FromQuery] string? nombre,
            [FromQuery] int? idUsuario)
        {
            return Ok(await _service.ListarAsync(nombre, idUsuario));
        }

        /// <summary>Devuelve los metadatos de un archivo.</summary>
        /// <param name="id">Id del archivo.</param>
        /// <response code="404">El archivo no existe.</response>
        // GET /archivos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivoDTO>> BuscarArchivo(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Sube un archivo. El propietario es el usuario del token.</summary>
        /// <param name="archivo">Archivo enviado como multipart/form-data.</param>
        /// <param name="ruta">Subcarpeta opcional donde guardarlo.</param>
        /// <response code="201">Archivo guardado.</response>
        /// <response code="400">Archivo vacio o subcarpeta invalida.</response>
        // POST /archivos?ruta={subcarpeta}
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ArchivoDTO>> SubirArchivo(IFormFile archivo, [FromQuery] string? ruta)
        {
            await using var contenido = archivo.OpenReadStream();

            var creado = await _service.SubirAsync(new ArchivoParaSubir
            {
                Contenido = contenido,
                NombreOriginal = archivo.FileName,
                Tipo = archivo.ContentType,
                Longitud = archivo.Length,
                Subcarpeta = ruta
            });

            return CreatedAtAction(nameof(BuscarArchivo), new { id = creado.Id }, creado);
        }

        /// <summary>Descarga un archivo como adjunto.</summary>
        /// <param name="id">Id del archivo.</param>
        /// <response code="403">El usuario no tiene permiso para verlo.</response>
        /// <response code="404">No existe el registro o el archivo fisico.</response>
        // GET /archivos/descargar/{id}
        [HttpGet("descargar/{id}")]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
            var archivo = await _service.ObtenerContenidoAsync(id);
            return File(archivo.Contenido, archivo.Tipo, archivo.NombreOriginal);
        }

        /// <summary>Muestra un archivo en linea; admite lectura parcial (por ejemplo, video).</summary>
        /// <param name="id">Id del archivo.</param>
        /// <response code="403">El usuario no tiene permiso para verlo.</response>
        /// <response code="404">No existe el registro o el archivo fisico.</response>
        // GET /archivos/ver/{id}
        [HttpGet("ver/{id}")]
        public async Task<IActionResult> VerArchivo(int id)
        {
            var archivo = await _service.ObtenerContenidoAsync(id);
            return File(archivo.Contenido, archivo.Tipo, enableRangeProcessing: true);
        }

        /// <summary>Corrige los metadatos de un archivo. Solo administradores.</summary>
        /// <param name="id">Id del archivo.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /archivos/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> ActualizarMetadatos(int id, [FromBody] ModificarArchivoDTO dto)
        {
            await _service.ActualizarMetadatosAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un archivo y su contenido fisico. Solo administradores.</summary>
        /// <param name="id">Id del archivo.</param>
        // DELETE /archivos/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarArchivo(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
