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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaArchivoDTO>>> ListarArchivosTarea(int idTarea)
        {
            var archivosDeTarea = await _context.TareaArchivos
                .Where(ta => ta.IdTarea == idTarea)
                .Join(_context.Archivos,
                    ta => ta.IdArchivo,
                    a => a.Id,
                    (ta, a) => new TareaArchivoDTO
                    {
                        IdTarea = ta.IdTarea,
                        IdArchivo = ta.IdArchivo,
                        NombreOriginal = a.NombreOriginal,
                        Ruta = a.Ruta
                    })
                .ToListAsync();

            return Ok(archivosDeTarea);
        }

        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincTareaArchivoDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe())
            {
                return Forbid("Solo los profesores o administradores pueden vincular archivos a las tareas.");
            }

            var existeTarea = await _context.Tareas.AnyAsync(t => t.Id == dto.IdTarea);
            var existeArchivo = await _context.Archivos.AnyAsync(a => a.Id == dto.IdArchivo);

            if (!existeTarea || !existeArchivo)
                return BadRequest("La tarea o el archivo especificado no existen.");

            var yaVinculado = await _context.TareaArchivos
                .AnyAsync(ta => ta.IdTarea == dto.IdTarea && ta.IdArchivo == dto.IdArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado a la tarea.");

            var relacion = new TareaArchivo
            {
                IdTarea = dto.IdTarea,
                IdArchivo = dto.IdArchivo
            };

            _context.TareaArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idTarea, [FromQuery] int idArchivo)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var registro = await _context.TareaArchivos
                .FirstOrDefaultAsync(ta => ta.IdTarea == idTarea && ta.IdArchivo == idArchivo);

            if (registro == null) return NotFound("No se encontró el vínculo entre la tarea y el archivo.");

            _context.TareaArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}