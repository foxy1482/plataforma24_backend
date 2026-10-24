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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> ListarMateriales(
            [FromQuery] int? idMateria)
        {
            var cons = _context.Materiales.AsQueryable();

            if (idMateria != null && idMateria != 0)
            {
                cons = cons.Where(m => m.IdMateria == idMateria);
            }

            var resultados = await cons.ToListAsync();

            return Ok(resultados.Select(m => MapearDTO(m)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDTO>> BuscarMaterial(int id)
        {
            var m = await _context.Materiales.FindAsync(id);
            if (m == null) return NotFound();

            return Ok(MapearDTO(m));
        }

        [HttpPost]
        public async Task<ActionResult<MaterialDTO>> CrearMaterial([FromBody] CrearMaterialDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = new Material
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                PublicadaDesde = dto.PublicadaDesde,
                IdMateria = dto.IdMateria,
            };


            _context.Materiales.Add(datos);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarMaterial), new { id = datos.Id }, dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarMaterial(int id, [FromBody] ModificarMaterialDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var material = await _context.Materiales.FindAsync(id);
            if (material == null) return NotFound($"No se encontró el material número {id}");


            if (dto.Titulo != null) material.Titulo = dto.Titulo;
            if (dto.Descripcion != null) material.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMaterial(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var material = await _context.Materiales.FindAsync(id);
            if (material == null) return NotFound();

            try
            {
                _context.Materiales.Remove(material);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("No se pudo eliminar la tarea");
            }
            return NoContent();
        }


        private static MaterialDTO MapearDTO(Material m)
        {
            return new MaterialDTO
            {
                Id = m.Id,
                Titulo = m.Titulo,
                Descripcion = m.Descripcion,
                PublicadaDesde = m.PublicadaDesde,
                IdMateria = m.IdMateria,
            };
        }
    }
}