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
        // GET /archivos?nombre={nombre}&legajo={numLegajo}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivoDTO>>> ListarArchivos(
            [FromQuery] string? nombre,
            [FromQuery] int? legajo
            )
        {
            var cons = _context.Archivos.AsQueryable();

            if (nombre != null)
            {
                cons = cons.Where(arc => arc.NombreOriginal == nombre);
            }

            if (legajo.HasValue)
            {
                cons = cons.Where(arc => arc.NumLegajo == legajo.Value);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(arc => MapearDTO(arc)));
        }

        // GET /archivos/{numero}
        [HttpGet("{numero}")]
        public async Task<ActionResult<ArchivoDTO>> BuscarArchivo(int numero)
        {
            var a = await _context.Archivos.FindAsync(numero);
            if (a == null) return NotFound();

            return Ok(MapearDTO(a));
        }

        // POST /archivos/
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ArchivoDTO>> SubirArchivo(IFormFile archivo, [FromQuery] string? ruta) // Quitamos el numLegajo de acá
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Archivo no seleccionado");

            // Sacamos el legajo del token del usuario actual
            int legajoUsuario = User.GetLegajo()!.Value;

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
                NumLegajo = legajoUsuario
            };

            _context.Archivos.Add(datos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarArchivo), new { numero = datos.Numero }, MapearDTO(datos));
        }

        // GET /archivos/descargar/{numero}
        [HttpGet("descargar/{numero}")]
        public async Task<IActionResult> DescargarArchivo(int numero)
        {
            var registro = await _context.Archivos.FindAsync(numero);
            if (registro == null) return NotFound("El archivo no existe en la base de datos.");

            var dueño = await _context.Usuarios.FindAsync(registro.NumLegajo);
            if (dueño == null) return NotFound("Dueño del archivo no encontrado.");

            var miLegajo = User.GetLegajo();
            var miRol = User.GetRol();

            // --- LÓGICA DE PERMISOS ---
            bool tieneAcceso = false;

            if (User.EsAdmin())
            {
                tieneAcceso = true; // Admin ve todo
            }
            else if (User.EsProfe())
            {
                if (dueño.NumRol < 2) tieneAcceso = true;
                if (registro.NumLegajo == miLegajo) tieneAcceso = true;
            }
            else
            {
                if (registro.NumLegajo == miLegajo) tieneAcceso = true;
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

        // PATCH /archivos/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarMetadatos(int numero, [FromBody] ModificarArchivoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var archivo = await _context.Archivos.FindAsync(numero);
            if (archivo == null) return NotFound();

            // Solo permitimos cambiar el nombre "lindo" o quizás el legajo si hubo un error
            if (!string.IsNullOrEmpty(dto.NombreOriginal))
            {
                archivo.NombreOriginal = dto.NombreOriginal;
            }

            if (dto.NumLegajo.HasValue)
            {
                archivo.NumLegajo = dto.NumLegajo.Value;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        // DELETE /archivos/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarArchivo(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var registro = await _context.Archivos.FindAsync(numero);
            if (registro == null) return NotFound();

            // 1. Intentar borrar el archivo físico del disco
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
        [HttpGet("ver/{numero}")]
        public async Task<IActionResult> VerArchivo(int numero)
        {
            var registro = await _context.Archivos.FindAsync(numero);
            if (registro == null) return NotFound();

            var dueño = await _context.Usuarios.FindAsync(registro.NumLegajo);
            if (dueño == null) return NotFound("Dueño del archivo no encontrado.");

            var miLegajo = User.GetLegajo();
            var miRol = User.GetRol();

            // --- LÓGICA DE PERMISOS ---
            bool tieneAcceso = false;

            if (User.EsAdmin())
            {
                tieneAcceso = true; // Admin ve todo
            }
            else if (User.EsProfe())
            {
                // Profe ve archivos si el dueño es un alumno (rol menor a 2)
                if (dueño.NumRol < 2) tieneAcceso = true;
                if (registro.NumLegajo == miLegajo) tieneAcceso = true;
            }
            else
            {
                if (registro.NumLegajo == miLegajo) tieneAcceso = true;
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
                Numero = a.Numero,
                NombreOriginal = a.NombreOriginal,
                GuidArchivo = a.GuidArchivo,
                Ruta = a.Ruta,
                Tipo = a.Tipo,
                FechaSubida = a.FechaSubida,
                NumLegajo = a.NumLegajo,
            };
        }
    }
}
