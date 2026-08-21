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

        // GET /cursos
        // GET /cursos?ano={año}&turno={turno}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoDTO>>> ListarCursos(
            [FromQuery] Year? ano,
            [FromQuery] Turno? turno)
        {
            // Empezamos con la consulta base
            var cons = _context.Cursos.AsQueryable();

            // Si el usuario mandó el año aplico el filtro
            if (ano.HasValue)
            {
                cons = cons.Where(c => c.Año == ano.Value);
            }

            // Si el usuario mandó el turno, le agrego el filtro
            if (turno.HasValue)
            {
                cons = cons.Where(c => c.Turno == turno.Value);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(c => MapearDTO(c)));
        }

        // GET /cursos/{numero}
        [HttpGet("{numero}")]
        public async Task<ActionResult<CursoDTO>> BuscarCurso(int numero)
        {
            var c = await _context.Cursos.FindAsync(numero);
            if (c == null) return NotFound();

            return Ok(MapearDTO(c));
        }

        // POST /cursos
        [HttpPost]
        public async Task<ActionResult<CursoDTO>> CrearCurso([FromBody] CursoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var existe = await _context.Cursos.FindAsync(dto.Numero);
            if (existe != null) return BadRequest();

            var datos = new Curso
            {
                Numero = dto.Numero,
                Año = dto.Año,
                Division = dto.Division,
                Turno = dto.Turno,
                Aula = dto.Aula,
                CicloLectivo = dto.CicloLectivo
            };

            _context.Cursos.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarCurso), new { numero = datos.Numero}, dto);
        }

        // PATCH /cursos/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarCurso(int numero, [FromBody] ModificarCursoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var curso = await _context.Cursos.FindAsync(numero);
            if (curso == null) return NotFound($"No se encontró el curso número {numero}");

            // Actualización parcial: solo se modifican los campos que vienen en el JSON
            if (dto.Año.HasValue) curso.Año = dto.Año.Value;
            if (dto.Division.HasValue) curso.Division = dto.Division.Value;
            if (dto.Turno.HasValue) curso.Turno = dto.Turno.Value;
            if (dto.Aula != null) curso.Aula = dto.Aula;
            if (dto.CicloLectivo.HasValue) curso.CicloLectivo = dto.CicloLectivo.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /cursos/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarCurso(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var curso = await _context.Cursos.FindAsync(numero);
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

        // Función auxiliar para no repetir código de mapeo
        private static CursoDTO MapearDTO(Curso c)
        {
            return new CursoDTO
            {
                Numero = c.Numero,
                Año = c.Año,
                Division = c.Division,
                Turno = c.Turno,
                Aula = c.Aula,
                CicloLectivo = c.CicloLectivo
            };
        }
    }
}
