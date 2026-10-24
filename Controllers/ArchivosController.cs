using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Archivos;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("archivos")]
    [Authorize]
    public class ArchivosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArchivosController(AppDbContext context)
        {
            _context = context;
        }

        // GET /archivos/
        // GET /archivos?nombre={nombre}&idUsuario={idUsuario}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivoDTO>>> ListarArchivos(
            [FromQuery] string? nombre,
            [FromQuery] int? idUsuario
            )
        {
            var cons = _context.Archivos.AsQueryable();

            if (nombre != null)
            {
                cons = cons.Where(arc => arc.NombreOriginal == nombre);
            }

            if (idUsuario.HasValue)
            {
                cons = cons.Where(arc => arc.IdUsuario == idUsuario.Value);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(arc => MapearDTO(arc)));
        }

        // GET /archivos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivoDTO>> BuscarArchivo(int id)
        {
            var a = await _context.Archivos.FindAsync(id);
            if (a == null) return NotFound();

            return Ok(MapearDTO(a));
        }

        // POST /archivos/
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ArchivoDTO>> SubirArchivo(IFormFile archivo, [FromQuery] string? ruta) // El id del usuario sale del token
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Archivo no seleccionado");

            // Sacamos el id del usuario actual del token
            int idUsuario = User.GetUserId()!.Value;

            string subCarpeta = ruta ?? "";
            var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subCarpeta);

            if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

            Guid guid = Guid.NewGuid();
            var nombreUnico = $"{guid}_{archivo.FileName}";
            var rutaCompletaDisco = Path.Combine(carpetaDestino, nombreUnico);

            using (var stream = new FileStream(rutaCompletaDisco, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var rutaFinal = Path.Combine(subCarpeta, nombreUnico);

            var datos = new Archivo
            {
                NombreOriginal = archivo.FileName,
                GuidArchivo = guid,
                Ruta = rutaFinal, // <--- Guardamos "subcarpeta/guid_archivo.jpg"
                Tipo = archivo.ContentType,
                FechaSubida = DateTime.UtcNow,
                IdUsuario = idUsuario
            };

            _context.Archivos.Add(datos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarArchivo), new { id = datos.Id }, MapearDTO(datos));
        }

        // GET /archivos/descargar/{id}
        [HttpGet("descargar/{id}")]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
            var registro = await _context.Archivos.FindAsync(id);
            if (registro == null) return NotFound("El archivo no existe en la base de datos.");

            var dueño = await _context.Usuarios.FindAsync(registro.IdUsuario);
            if (dueño == null) return NotFound("Dueño del archivo no encontrado.");

            var miId = User.GetUserId();
            var miRol = User.GetRol();

            // --- LÓGICA DE PERMISOS ---
            bool tieneAcceso = false;

            if (User.EsAdmin())
            {
                tieneAcceso = true; // Admin ve todo
            }
            else if (User.EsProfe())
            {
                if (dueño.IdRol < 2) tieneAcceso = true;
                if (registro.IdUsuario == miId) tieneAcceso = true;
            }
            else
            {
                if (registro.IdUsuario == miId) tieneAcceso = true;
            }

            if (!tieneAcceso) return Forbid("No tenés permiso para ver este archivo.");

            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var rutaCompleta = Path.Combine(rutaCarpeta, registro.Ruta);

            // 3. Verificar si el archivo existe realmente en el disco
            if (!System.IO.File.Exists(rutaCompleta))
                return NotFound("El archivo físico fue eliminado o movido.");

            // 4. Determinar el tipo de contenido (MIME Type)
            // registro.Tipo ya lo guardamos al subir (ej: application/pdf)

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaCompleta);

            // 5. Devolver el archivo con su nombre original
            return File(bytes, registro.Tipo, registro.NombreOriginal);
        }

        // PATCH /archivos/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarMetadatos(int id, [FromBody] ModificarArchivoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var archivo = await _context.Archivos.FindAsync(id);
            if (archivo == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.NombreOriginal))
            {
                archivo.NombreOriginal = dto.NombreOriginal;
            }

            if (dto.IdUsuario.HasValue)
            {
                archivo.IdUsuario = dto.IdUsuario.Value;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarArchivo(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var registro = await _context.Archivos.FindAsync(id);
            if (registro == null) return NotFound();

            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var rutaCompleta = Path.Combine(rutaCarpeta, registro.Ruta);

            try
            {
                if (System.IO.File.Exists(rutaCompleta))
                {
                    System.IO.File.Delete(rutaCompleta);
                }
            }
            catch (IOException ex)
            {
                return StatusCode(500, "El archivo está en uso y no se pudo eliminar del disco.");
            }

            _context.Archivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("ver/{id}")]
        public async Task<IActionResult> VerArchivo(int id)
        {
            var registro = await _context.Archivos.FindAsync(id);
            if (registro == null) return NotFound();

            var dueño = await _context.Usuarios.FindAsync(registro.IdUsuario);
            if (dueño == null) return NotFound("Dueño del archivo no encontrado.");

            var miId = User.GetUserId();
            var miRol = User.GetRol();

            bool tieneAcceso = false;

            if (User.EsAdmin())
            {
                tieneAcceso = true; 
            }
            else if (User.EsProfe())
            {
                if (dueño.IdRol < 2) tieneAcceso = true;
                if (registro.IdUsuario == miId) tieneAcceso = true;
            }
            else
            {
                if (registro.IdUsuario == miId) tieneAcceso = true;
            }

            if (!tieneAcceso) return Forbid("No tenés permiso para ver este archivo.");

            var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", registro.Ruta);
            if (!System.IO.File.Exists(rutaCompleta)) return NotFound();

            var fileStream = new FileStream(rutaCompleta, FileMode.Open, FileAccess.Read);

            return File(fileStream, registro.Tipo, enableRangeProcessing: true);
        }

        private static ArchivoDTO MapearDTO(Archivo a)
        {
            return new ArchivoDTO
            {
                Id = a.Id,
                NombreOriginal = a.NombreOriginal,
                GuidArchivo = a.GuidArchivo,
                Ruta = a.Ruta,
                Tipo = a.Tipo,
                FechaSubida = a.FechaSubida,
                IdUsuario = a.IdUsuario,
            };
        }
    }
}