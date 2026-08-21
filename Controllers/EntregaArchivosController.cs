using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("/entregas/archivos")]
    [Authorize]
    public class EntregaArchivosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EntregaArchivosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntregaArchivoDTO>>> ListarArchivosEntrega(int numero)
        {
            var entrega = await _context.Entregas.FindAsync(numero);
            if (entrega == null) return NotFound("La entrega especificada no existe.");

            // REGLA: Un alumno solo puede ver los archivos de SUS propias entregas.
            if (!User.EsAdmin() && !User.EsProfe() && !User.TienePermiso(entrega.NumLegajo))
            {
                return Forbid("No tenés permiso para ver los archivos de esta entrega.");
            }

            var archivosEntrega = await _context.EntregaArchivos
                .Where(ta => ta.NumEntrega == numero)
                .Join(_context.Archivos,
                    ta => ta.NumArchivo,
                    a => a.Numero,
                    (ta, a) => new EntregaArchivoDTO
                    {
                        NumEntrega = ta.NumEntrega,
                        NumArchivo = ta.NumArchivo,
                        NombreOriginal = a.NombreOriginal,
                        Ruta = a.Ruta
                    })
                .ToListAsync();

            return Ok(archivosEntrega);
        }

        // POST /entregas/archivos/
        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincEntregaArchivoDTO dto)
        {
            var entrega = await _context.Entregas.FindAsync(dto.NumEntrega);
            var archivo = await _context.Archivos.FindAsync(dto.NumArchivo);

            if (entrega == null || archivo == null)
                return BadRequest("La entrega o el archivo especificado no existen.");

            // el usuario que vincula debe ser el dueño de la entrega
            if (!User.EsAdmin() && !User.TienePermiso(entrega.NumLegajo))
            {
                return Forbid("Solo podés agregarle archivos a tu propia entrega.");
            }
            // el usuario que vincula debe ser el dueño del ARCHIVO subido
            if (!User.EsAdmin() && archivo.NumLegajo != User.GetLegajo())
            {
                return BadRequest("No podés vincular un archivo que no te pertenece.");
            }

            var yaVinculado = await _context.EntregaArchivos
                .AnyAsync(ta => ta.NumEntrega == dto.NumEntrega && ta.NumArchivo == dto.NumArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado a la entrega.");

            var relacion = new EntregaArchivo
            {
                NumEntrega = dto.NumEntrega,
                NumArchivo = dto.NumArchivo
            };

            _context.EntregaArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /entregas/archivos/
        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int numEntrega, [FromQuery] int numArchivo)
        {
            var registro = await _context.EntregaArchivos
                .FirstOrDefaultAsync(ta => ta.NumEntrega == numEntrega && ta.NumArchivo == numArchivo);

            var infoEntrega = await _context.Entregas.FindAsync(numEntrega);
            if (!User.EsAdmin() && (infoEntrega == null || !User.TienePermiso(infoEntrega.NumLegajo)))
            {
                return Forbid("No tenés permiso para modificar esta entrega.");
            }

            var infoArchivo = await _context.Archivos.FindAsync(numArchivo);
            if (!User.EsAdmin() && infoArchivo != null && infoArchivo.NumLegajo != User.GetLegajo())
            {
                return Forbid("No podés quitar un archivo que no subiste vos.");
            }

            if (registro == null) return NotFound("No se encontró el vínculo entre la entrega y el archivo.");

            _context.EntregaArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /entregas/archivos/{numero}
        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarTodosLosArchivosDeEntrega(int numero)
        {
            var entrega = await _context.Entregas.FindAsync(numero);
            if (entrega == null) return NotFound("La entrega no existe.");

            if (!User.EsAdmin() && !User.TienePermiso(entrega.NumLegajo))
            {
                return Forbid("No podés modificar los archivos de esta entrega.");
            }

            var registrosABorrar = await _context.EntregaArchivos
                .Where(ea => ea.NumEntrega == numero)
                .ToListAsync();

            if (registrosABorrar.Any())
            {
                _context.EntregaArchivos.RemoveRange(registrosABorrar);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
