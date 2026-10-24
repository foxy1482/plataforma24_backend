using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("/materiales/archivos/")]
    [Authorize]
    public class MaterialArchivosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaterialArchivosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialArchivosDTO>>> ListarArchivosMaterial(int idMaterial)
        {
            var archivosMaterial = await _context.MaterialArchivos
                .Where(ta => ta.IdMaterial == idMaterial)
                .Join(_context.Archivos,
                    ta => ta.IdArchivo,
                    a => a.Id,
                    (ta, a) => new MaterialArchivosDTO
                    {
                        IdMaterial = ta.IdMaterial,
                        IdArchivo = ta.IdArchivo,
                        NombreOriginal = a.NombreOriginal,
                        Ruta = a.Ruta
                    })
                .ToListAsync();

            return Ok(archivosMaterial);
        }

        // POST /materiales/archivos/
        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincMaterialArchivoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var existeMaterial = await _context.Materiales.AnyAsync(t => t.Id == dto.IdMaterial);
            var existeArchivo = await _context.Archivos.AnyAsync(a => a.Id == dto.IdArchivo);

            if (!existeMaterial || !existeArchivo)
                return BadRequest("El material o el archivo especificado no existen.");

            var yaVinculado = await _context.MaterialArchivos
                .AnyAsync(ta => ta.IdMaterial == dto.IdMaterial && ta.IdArchivo == dto.IdArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado al material.");

            var relacion = new MaterialArchivo
            {
                IdMaterial = dto.IdMaterial,
                IdArchivo = dto.IdArchivo
            };

            _context.MaterialArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /materiales/archivos/
        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idMaterial, [FromQuery] int idArchivo)
        {
            if (!User.EsAdmin()) return Forbid();

            var registro = await _context.MaterialArchivos
                .FirstOrDefaultAsync(ta => ta.IdMaterial == idMaterial && ta.IdArchivo == idArchivo);

            if (registro == null) return NotFound("No se encontró el vínculo entre el material y el archivo.");

            _context.MaterialArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}