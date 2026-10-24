using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Entrega;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("entregas")]
    [Authorize]
    public class EntregasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EntregasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("alumno/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> BuscarPorAlumno(int idUsuario)
        {
            if (!User.TienePermiso(idUsuario)) return Forbid();

            var entregas = await _context.Entregas
                .Where(e => e.IdUsuario == idUsuario)
                .ToListAsync();
            return Ok(entregas);
        }

        [HttpGet("tarea/{idTarea}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> ListarPorTarea(int idTarea)
        {
            if (!User.EsAdmin()) return Forbid();

            var entregas = await _context.Entregas
                .Where(e => e.IdTarea == idTarea)
                .ToListAsync();
            return Ok(entregas);
        }

        [HttpGet("/materias/{idMateria}/entregas/alumno/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> ListarPorMateriaYAlumno(int idMateria, int idUsuario)
        {
            if (!User.TienePermiso(idUsuario)) return Forbid();

            var entregas = await _context.Entregas
                .Join(_context.Tareas,
                    e => e.IdTarea,
                    t => t.Id,
                    (e, t) => new { Entrega = e, Tarea = t })
                .Where(x => x.Tarea.IdMateria == idMateria && x.Entrega.IdUsuario == idUsuario)
                .Select(x => x.Entrega)
                .ToListAsync();

            return Ok(entregas);
        }

        [HttpPost]
        public async Task<ActionResult<Entrega>> RealizarEntrega([FromBody] EntregaDTO dto)
        {
            int idUsuario = User.GetUserId()!.Value;

            var tareaExiste = await _context.Tareas.AnyAsync(t => t.Id == dto.IdTarea);
            if (!tareaExiste) return BadRequest("La tarea especificada no existe.");

            var existe = await _context.Entregas
                .AnyAsync(e => e.IdTarea == dto.IdTarea && e.IdUsuario == idUsuario);
            if (existe) return BadRequest("Ya realizaste una entrega para esta tarea.");

            var nuevaEntrega = new Entrega
            {
                IdTarea = dto.IdTarea,
                IdUsuario = idUsuario,
                FechaEntrega = DateTime.UtcNow, 
            };

            _context.Entregas.Add(nuevaEntrega);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarPorAlumno), new { idUsuario = nuevaEntrega.IdUsuario }, nuevaEntrega);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ModificarEntrega(int id, [FromBody] ModificarEntregaDTO dto)
        {
            var entrega = await _context.Entregas.FindAsync(id);
            if (entrega == null) return NotFound();

            if (!User.TienePermiso(entrega.IdUsuario)) return Forbid();

            if (dto.FechaEntrega.HasValue)
                entrega.FechaEntrega = dto.FechaEntrega.Value;

            if (dto.Calificacion != null && User.EsAdmin())
                entrega.Calificacion = dto.Calificacion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEntrega(int id)
        {
            var entrega = await _context.Entregas.FindAsync(id);
            if (entrega == null) return NotFound();

            if (!User.TienePermiso(entrega.IdUsuario)) return Forbid();

            _context.Entregas.Remove(entrega);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}