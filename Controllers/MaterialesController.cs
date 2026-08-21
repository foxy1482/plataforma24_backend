using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Material;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("materiales")]
    [Authorize]
    public class MaterialesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MaterialesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /materiales
        // GET /materiales?materia={numMateria}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> ListarMateriales(
            [FromQuery] int? numMateria)
        {
            var cons = _context.Materiales.AsQueryable();

            if (numMateria != null && numMateria != 0)
            {
                cons = cons.Where(m => m.NumMateria == numMateria);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(m => MapearDTO(m)));
        }

        // GET /materiales/{numero}
        [HttpGet("{numero}")]
        public async Task<ActionResult<MaterialDTO>> BuscarMaterial(int numero)
        {
            var m = await _context.Materiales.FindAsync(numero);
            if (m == null) return NotFound();

            return Ok(MapearDTO(m));
        }

        // POST /materiales
        [HttpPost]
        public async Task<ActionResult<MaterialDTO>> CrearMaterial([FromBody] CrearMaterialDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = new Material
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                PublicadaDesde = dto.PublicadaDesde,
                NumMateria = dto.NumMateria,
            };
           

            _context.Materiales.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarMaterial), new { numero = datos.Numero }, dto);
        }

        // PATCH /materiales/{numero}
        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarMaterial(int numero, [FromBody] ModificarMaterialDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var material = await _context.Materiales.FindAsync(numero);
            if (material == null) return NotFound($"No se encontró el material número {numero}");


            if (dto.Titulo != null) material.Titulo = dto.Titulo;
            if (dto.Descripcion != null) material.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /materiales/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarMaterial(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var material = await _context.Materiales.FindAsync(numero);
            if (material == null) return NotFound();

            try
            {
                _context.Materiales.Remove(material);
                await _context.SaveChangesAsync();
            } catch (DbUpdateException)
            {
                return BadRequest("No se pudo eliminar la tarea");
            }
            return NoContent();
        }


        private static MaterialDTO MapearDTO(Material m)
        {
            return new MaterialDTO
            {
                Numero = m.Numero,
                Titulo = m.Titulo,
                Descripcion = m.Descripcion,
                PublicadaDesde = m.PublicadaDesde,
                NumMateria = m.NumMateria,
            };
        }
    }
}
