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
        public async Task<ActionResult<IEnumerable<MaterialArchivosDTO>>> ListarArchivosMaterial(int numero)
        {
            var archivosMaterial = await _context.MaterialArchivos
                .Where(ta => ta.NumMaterial == numero)
                .Join(_context.Archivos,
                    ta => ta.NumArchivo,
                    a => a.Numero,
                    (ta, a) => new MaterialArchivosDTO
                    {
                        NumMaterial = ta.NumMaterial,
                        NumArchivo = ta.NumArchivo,
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

            var existeMaterial = await _context.Materiales.AnyAsync(t => t.Numero == dto.NumMaterial);
            var existeArchivo = await _context.Archivos.AnyAsync(a => a.Numero == dto.NumArchivo);

            if (!existeMaterial || !existeArchivo)
                return BadRequest("El material o el archivo especificado no existen.");

            var yaVinculado = await _context.MaterialArchivos
                .AnyAsync(ta => ta.NumMaterial == dto.NumMaterial && ta.NumArchivo == dto.NumArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado al material.");

            var relacion = new MaterialArchivo
            {
                NumMaterial = dto.NumMaterial,
                NumArchivo = dto.NumArchivo
            };

            _context.MaterialArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /materiales/archivos/
        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int numMaterial, [FromQuery] int numArchivo)
        {
            if (!User.EsAdmin()) return Forbid();

            var registro = await _context.MaterialArchivos
                .FirstOrDefaultAsync(ta => ta.NumMaterial == numMaterial && ta.NumArchivo == numArchivo);

            if (registro == null) return NotFound("No se encontró el vínculo entre el material y el archivo.");

            _context.MaterialArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
