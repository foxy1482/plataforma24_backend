using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("/tareas/archivos")]
    [Authorize]
    public class TareaArchivosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareaArchivosController(AppDbContext context)
        {
            _context = context;
        }

        // GET /tareas/archivos/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaArchivoDTO>>> ListarArchivosTarea(int numero)
        {
            var archivosDeTarea = await _context.TareaArchivos
                .Where(ta => ta.NumTarea == numero)
                .Join(_context.Archivos,
                    ta => ta.NumArchivo,
                    a => a.Numero,
                    (ta, a) => new TareaArchivoDTO
                    {
                        NumTarea = ta.NumTarea,
                        NumArchivo = ta.NumArchivo,
                        NombreOriginal = a.NombreOriginal,
                        Ruta = a.Ruta
                    })
                .ToListAsync();

            return Ok(archivosDeTarea);
        }

        // POST /tareas/archivos/
        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincTareaArchivoDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe())
            {
                return Forbid("Solo los profesores o administradores pueden vincular archivos a las tareas.");
            }

            var existeTarea = await _context.Tareas.AnyAsync(t => t.Numero == dto.NumTarea);
            var existeArchivo = await _context.Archivos.AnyAsync(a => a.Numero == dto.NumArchivo);

            if (!existeTarea || !existeArchivo)
                return BadRequest("La tarea o el archivo especificado no existen.");

            var yaVinculado = await _context.TareaArchivos
                .AnyAsync(ta => ta.NumTarea == dto.NumTarea && ta.NumArchivo == dto.NumArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado a la tarea.");

            var relacion = new TareaArchivo
            {
                NumTarea = dto.NumTarea,
                NumArchivo = dto.NumArchivo
            };

            _context.TareaArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /tareas/archivos/
        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int numTarea, [FromQuery] int numArchivo)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var registro = await _context.TareaArchivos
                .FirstOrDefaultAsync(ta => ta.NumTarea == numTarea && ta.NumArchivo == numArchivo);

            if (registro == null) return NotFound("No se encontró el vínculo entre la tarea y el archivo.");

            _context.TareaArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
