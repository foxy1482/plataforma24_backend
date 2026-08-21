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

        // GET /materias/
        // GET /materias?profesor={numero}&curso={numero}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaDTO>>> ListarMaterias(
            [FromQuery] int? profesor,
            [FromQuery] int? curso)
        {
            var cons = _context.Materias.AsQueryable();

            if (profesor != null && curso != 0)
            {
                cons = cons.Where(m => m.NumProfesor == profesor);
            }

            if (curso != null && curso != 0)
            {
                cons = cons.Where(m => m.NumCurso == curso);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(m => MapearDTO(m)));
        }

        // GET /materias/{numero}
        [HttpGet("{numero}")]
        public async Task<ActionResult<MateriaDTO>> BuscarMateria(int numero)
        {
            var m = await _context.Materias.FindAsync(numero);
            if (m == null) return NotFound();

            return Ok(MapearDTO(m));
        }


        // POST /materias/
        [HttpPost]
        public async Task<ActionResult<MateriaDTO>> CrearMateria([FromBody] MateriaDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var existe = await _context.Materias.FindAsync(dto.Numero);
            if (existe != null) return BadRequest();

            var datos = new Materia
            {
                Numero = dto.Numero,
                Nombre = dto.Nombre,
                NumProfesor = dto.NumProfesor,
                NumCurso = dto.NumCurso,
            };

            _context.Materias.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarMateria), new { numero = datos.Numero }, dto);
        }

        // PATCH /materias/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarMateria(int numero, [FromBody] ModificarMateriaDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var materia = await _context.Materias.FindAsync(numero);
            if (materia == null) return NotFound($"No se encontró la materia número {numero}");

            if (dto.Nombre != null) materia.Nombre = dto.Nombre;
            if (dto.NumProfesor.HasValue) materia.NumProfesor = dto.NumProfesor.Value;
            if (dto.NumCurso.HasValue) materia.NumCurso = dto.NumCurso.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /materias/{numero}
        [HttpDelete]
        public async Task<IActionResult> EliminarMateria(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var materia = await _context.Materias.FindAsync(numero);
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

        // Función auxiliar
        private static MateriaDTO MapearDTO(Materia m)
        {
            return new MateriaDTO
            {
                Numero = m.Numero,
                Nombre = m.Nombre,
                NumProfesor = m.NumProfesor,
                NumCurso = m.NumCurso
            };
        }
    }
}
