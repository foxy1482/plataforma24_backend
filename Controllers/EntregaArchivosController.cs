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
        public async Task<ActionResult<IEnumerable<EntregaArchivoDTO>>> ListarArchivosEntrega(int idEntrega)
        {
            var entrega = await _context.Entregas.FindAsync(idEntrega);
            if (entrega == null) return NotFound("La entrega especificada no existe.");

            if (!User.EsAdmin() && !User.EsProfe() && !User.TienePermiso(entrega.IdUsuario))
            {
                return Forbid("No tenés permiso para ver los archivos de esta entrega.");
            }

            var archivosEntrega = await _context.EntregaArchivos
                .Where(ta => ta.IdEntrega == idEntrega)
                .Join(_context.Archivos,
                    ta => ta.IdArchivo,
                    a => a.Id,
                    (ta, a) => new EntregaArchivoDTO
                    {
                        IdEntrega = ta.IdEntrega,
                        IdArchivo = ta.IdArchivo,
                        NombreOriginal = a.NombreOriginal,
                        Ruta = a.Ruta
                    })
                .ToListAsync();

            return Ok(archivosEntrega);
        }

        [HttpPost]
        public async Task<IActionResult> VincularArchivo([FromBody] VincEntregaArchivoDTO dto)
        {
            var entrega = await _context.Entregas.FindAsync(dto.IdEntrega);
            var archivo = await _context.Archivos.FindAsync(dto.IdArchivo);

            if (entrega == null || archivo == null)
                return BadRequest("La entrega o el archivo especificado no existen.");

            if (!User.EsAdmin() && !User.TienePermiso(entrega.IdUsuario))
            {
                return Forbid("Solo podés agregarle archivos a tu propia entrega.");
            }
            if (!User.EsAdmin() && archivo.IdUsuario != User.GetUserId())
            {
                return BadRequest("No podés vincular un archivo que no te pertenece.");
            }

            var yaVinculado = await _context.EntregaArchivos
                .AnyAsync(ta => ta.IdEntrega == dto.IdEntrega && ta.IdArchivo == dto.IdArchivo);
            if (yaVinculado) return BadRequest("Este archivo ya está vinculado a la entrega.");

            var relacion = new EntregaArchivo
            {
                IdEntrega = dto.IdEntrega,
                IdArchivo = dto.IdArchivo
            };

            _context.EntregaArchivos.Add(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DesvincularArchivo([FromQuery] int idEntrega, [FromQuery] int idArchivo)
        {
            var registro = await _context.EntregaArchivos
                .FirstOrDefaultAsync(ta => ta.IdEntrega == idEntrega && ta.IdArchivo == idArchivo);

            var infoEntrega = await _context.Entregas.FindAsync(idEntrega);
            if (!User.EsAdmin() && (infoEntrega == null || !User.TienePermiso(infoEntrega.IdUsuario)))
            {
                return Forbid("No tenés permiso para modificar esta entrega.");
            }

            var infoArchivo = await _context.Archivos.FindAsync(idArchivo);
            if (!User.EsAdmin() && infoArchivo != null && infoArchivo.IdUsuario != User.GetUserId())
            {
                return Forbid("No podés quitar un archivo que no subiste vos.");
            }

            if (registro == null) return NotFound("No se encontró el vínculo entre la entrega y el archivo.");

            _context.EntregaArchivos.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{idEntrega}")]
        public async Task<IActionResult> EliminarTodosLosArchivosDeEntrega(int idEntrega)
        {
            var entrega = await _context.Entregas.FindAsync(idEntrega);
            if (entrega == null) return NotFound("La entrega no existe.");

            if (!User.EsAdmin() && !User.TienePermiso(entrega.IdUsuario))
            {
                return Forbid("No podés modificar los archivos de esta entrega.");
            }

            var registrosABorrar = await _context.EntregaArchivos
                .Where(ea => ea.IdEntrega == idEntrega)
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