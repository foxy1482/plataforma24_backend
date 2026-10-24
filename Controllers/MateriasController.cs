using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Materia;
using Plataforma24API.DTOs.Materias;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("materias")]
    [Authorize]
    public class MateriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MateriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaDTO>>> ListarMaterias(
            [FromQuery] int? profesor,
            [FromQuery] int? curso)
        {
            var cons = _context.Materias.AsQueryable();

            if (profesor != null && profesor != 0)
            {
                cons = cons.Where(m => m.IdProfesor == profesor);
            }

            if (curso != null && curso != 0)
            {
                cons = cons.Where(m => m.IdCurso == curso);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(m => MapearDTO(m)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MateriaDTO>> BuscarMateria(int id)
        {
            var m = await _context.Materias.FindAsync(id);
            if (m == null) return NotFound();

            return Ok(MapearDTO(m));
        }


        [HttpPost]
        public async Task<ActionResult<MateriaDTO>> CrearMateria([FromBody] MateriaDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var duplicada = await _context.Materias.AnyAsync(m => m.IdCurso == dto.IdCurso && m.Nombre == dto.Nombre);
            if (duplicada) return BadRequest("Ya existe una materia con ese nombre en el curso.");

            var datos = new Materia
            {
                Nombre = dto.Nombre,
                IdProfesor = dto.IdProfesor,
                IdCurso = dto.IdCurso,
            };

            _context.Materias.Add(datos);

            await _context.SaveChangesAsync();

            dto.Id = datos.Id;   
            return CreatedAtAction(nameof(BuscarMateria), new { id = datos.Id }, dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarMateria(int id, [FromBody] ModificarMateriaDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return NotFound($"No se encontró la materia número {id}");

            if (dto.Nombre != null) materia.Nombre = dto.Nombre;
            if (dto.IdProfesor.HasValue) materia.IdProfesor = dto.IdProfesor.Value;
            if (dto.IdCurso.HasValue) materia.IdCurso = dto.IdCurso.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMateria(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return NotFound();

            try
            {
                _context.Materias.Remove(materia);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se puede eliminar la materia porque tiene profesores o cursos asociados.");
            }

            return NoContent();
        }

        private static MateriaDTO MapearDTO(Materia m)
        {
            return new MateriaDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                IdProfesor = m.IdProfesor,
                IdCurso = m.IdCurso
            };
        }
    }
}