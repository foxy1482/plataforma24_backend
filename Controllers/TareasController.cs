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

        // GET /tareas
        // GET /tareas?materia={numMateria}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TareaDTO>>> ListarTareas(
            [FromQuery] int? materia)
        {
            var cons = _context.Tareas.AsQueryable();

            if (materia != null && materia != 0)
            {
                cons = cons.Where(t => t.NumMateria == materia);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(t => MapearDTO(t)));
        }

        // GET /tareas/{numero}
        [HttpGet("{numero}")]
        public async Task<ActionResult<TareaDTO>> BuscarTarea(int numero)
        {
            var t = await _context.Tareas.FindAsync(numero);
            if (t == null) return NotFound();

            return Ok(MapearDTO(t));
        }

        // POST /tareas
        [HttpPost]
        public async Task<ActionResult<TareaDTO>> CrearTarea([FromBody] CrearTareaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var datos = new Tarea
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaEntrega = dto.FechaEntrega,
                PublicadaDesde = dto.PublicadaDesde,
                NumMateria = dto.NumMateria,
            };

            _context.Tareas.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarTarea), new { numero = datos.Numero }, dto);

        }

        // PATCH /tareas/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarTarea(int numero, [FromBody] ModificarTareaDTO dto)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var tarea = await _context.Tareas.FindAsync(numero);
            if (tarea == null) return NotFound($"No se encontró la tarea número {numero}");

            if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
            if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
            if (dto.FechaEntrega.HasValue) tarea.FechaEntrega = dto.FechaEntrega.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /tareas/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarTarea(int numero)
        {
            if (!User.EsAdmin() && !User.EsProfe()) return Forbid();

            var tarea = await _context.Tareas.FindAsync(numero);
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
                Numero = t.Numero,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                FechaEntrega = t.FechaEntrega,
                PublicadaDesde = t.PublicadaDesde,
                NumMateria = t.NumMateria,
            };
        }
    }
}
