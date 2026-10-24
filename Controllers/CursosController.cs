using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Cursos;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("cursos")]
    [Authorize]
    public class CursosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CursosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoDTO>>> ListarCursos(
            [FromQuery] Year? ano,
            [FromQuery] Turno? turno)
        {
            var cons = _context.Cursos.AsQueryable();

            if (ano.HasValue)
            {
                cons = cons.Where(c => c.Año == ano.Value);
            }

            if (turno.HasValue)
            {
                cons = cons.Where(c => c.Turno == turno.Value);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(c => MapearDTO(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDTO>> BuscarCurso(int id)
        {
            var c = await _context.Cursos.FindAsync(id);
            if (c == null) return NotFound();

            return Ok(MapearDTO(c));
        }

        [HttpPost]
        public async Task<ActionResult<CursoDTO>> CrearCurso([FromBody] CursoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var duplicado = await _context.Cursos.AnyAsync(c =>
                c.Año == dto.Año && c.Division == dto.Division &&
                c.Turno == dto.Turno && c.CicloLectivo == dto.CicloLectivo);
            if (duplicado) return BadRequest("Ya existe un curso con ese año, división, turno y ciclo lectivo.");

            var datos = new Curso
            {
                Año = dto.Año,
                Division = dto.Division,
                Turno = dto.Turno,
                Aula = dto.Aula,
                CicloLectivo = dto.CicloLectivo
            };

            _context.Cursos.Add(datos);

            await _context.SaveChangesAsync();

            dto.Id = datos.Id;  
            return CreatedAtAction(nameof(BuscarCurso), new { id = datos.Id }, dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarCurso(int id, [FromBody] ModificarCursoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound($"No se encontró el curso número {id}");

            if (dto.Año.HasValue) curso.Año = dto.Año.Value;
            if (dto.Division.HasValue) curso.Division = dto.Division.Value;
            if (dto.Turno.HasValue) curso.Turno = dto.Turno.Value;
            if (dto.Aula != null) curso.Aula = dto.Aula;
            if (dto.CicloLectivo.HasValue) curso.CicloLectivo = dto.CicloLectivo.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCurso(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null) return NotFound();

            try
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar el curso porque tiene alumnos o registros asociados.");
            }

            return NoContent();
        }

        private static CursoDTO MapearDTO(Curso c)
        {
            return new CursoDTO
            {
                Id = c.Id,
                Año = c.Año,
                Division = c.Division,
                Turno = c.Turno,
                Aula = c.Aula,
                CicloLectivo = c.CicloLectivo
            };
        }
    }
}