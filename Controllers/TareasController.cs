using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("tareas")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaDTO>>> ListarTareas(
            [FromQuery] int? materia)
        {
            var cons = _context.Tareas.AsQueryable();

            if (materia != null && materia != 0)
            {
                cons = cons.Where(t => t.IdMateria == materia);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(t => MapearDTO(t)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TareaDTO>> BuscarTarea(int id)
        {
            var t = await _context.Tareas.FindAsync(id);
            if (t == null) return NotFound();

            return Ok(MapearDTO(t));
        }

        [HttpPost]
        public async Task<ActionResult<TareaDTO>> CrearTarea([FromBody] CrearTareaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            if (dto.FechaEntrega < dto.PublicadaDesde)
                return BadRequest("La fecha de entrega no puede ser anterior a la fecha de publicación.");

            var datos = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaEntrega = dto.FechaEntrega,
                PublicadaDesde = dto.PublicadaDesde,
                IdMateria = dto.IdMateria,
            };

            _context.Tareas.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarTarea), new { id = datos.Id }, dto);

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarTarea(int id, [FromBody] ModificarTareaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound($"No se encontró la tarea número {id}");

            if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
            if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
            if (dto.FechaEntrega.HasValue) tarea.FechaEntrega = dto.FechaEntrega.Value;

            if (tarea.FechaEntrega < tarea.PublicadaDesde)
                return BadRequest("La fecha de entrega no puede ser anterior a la fecha de publicación.");

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null) return NotFound();

            try
            {
                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo eliminar la tarea");
            }
            return NoContent();
        }

        private static TareaDTO MapearDTO(Tarea t)
        {
            return new TareaDTO
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                FechaEntrega = t.FechaEntrega,
                PublicadaDesde = t.PublicadaDesde,
                IdMateria = t.IdMateria,
            };
        }
    }
}