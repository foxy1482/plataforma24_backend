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

        // GET /entregas/alumno/{legajo}
        [HttpGet("alumno/{legajo}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> BuscarPorAlumno(int legajo)
        {
            if (!User.TienePermiso(legajo)) return Forbid();

            var entregas = await _context.Entregas
                .Where(e => e.NumLegajo == legajo)
                .ToListAsync();
            return Ok(entregas);
        }

        // GET /entregas/tarea/{numero}
        [HttpGet("tarea/{numero}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> ListarPorTarea(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var entregas = await _context.Entregas
                .Where(e => e.NumTarea == numero)
                .ToListAsync();
            return Ok(entregas);
        }

        // GET /materias/{numMateria}/entregas/alumno/{legajo}
        [HttpGet("/materias/{numMateria}/entregas/alumno/{legajo}")]
        public async Task<ActionResult<IEnumerable<Entrega>>> ListarPorMateriaYAlumno(int numMateria, int legajo)
        {
            if (!User.TienePermiso(legajo)) return Forbid();

            // join con tareas
            var entregas = await _context.Entregas
                .Join(_context.Tareas,
                    e => e.NumTarea,
                    t => t.Numero,
                    (e, t) => new { Entrega = e, Tarea = t })
                .Where(x => x.Tarea.NumMateria == numMateria && x.Entrega.NumLegajo == legajo)
                .Select(x => x.Entrega)
                .ToListAsync();

            return Ok(entregas);
        }

        // POST /entregas/
        [HttpPost]
        public async Task<ActionResult<Entrega>> RealizarEntrega([FromBody] EntregaDTO dto)
        {
            int legajo = User.GetLegajo()!.Value;

            // Evitar entregas duplicadas para la misma tarea si fuera necesario
            var existe = await _context.Entregas
                .AnyAsync(e => e.NumTarea == dto.NumTarea && e.NumLegajo == legajo);
            if (existe) return BadRequest("Ya realizaste una entrega para esta tarea.");

            var nuevaEntrega = new Entrega
            {
                NumTarea = dto.NumTarea,
                NumLegajo = legajo,
                FechaEntrega = DateTime.UtcNow, // Automática
            };

            _context.Entregas.Add(nuevaEntrega);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarPorAlumno), new { legajo = nuevaEntrega.NumLegajo }, nuevaEntrega);
        }

        // PATCH /entregas/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ModificarEntrega(int numero, [FromBody] ModificarEntregaDTO dto)
        {
            var entrega = await _context.Entregas.FindAsync(numero);
            if (entrega == null) return NotFound();

            if (!User.TienePermiso(entrega.NumLegajo)) return Forbid();

            if (dto.FechaEntrega.HasValue)
                entrega.FechaEntrega = dto.FechaEntrega.Value;

            if (dto.Calificacion != null && User.EsAdmin())
                entrega.Calificacion = dto.Calificacion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /entregas/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarEntrega(int numero)
        {
            var entrega = await _context.Entregas.FindAsync(numero);
            if (entrega == null) return NotFound();

            if (!User.TienePermiso(entrega.NumLegajo)) return Forbid();

            _context.Entregas.Remove(entrega);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
