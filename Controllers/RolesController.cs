using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Rol;
using Plataforma24API.Auth;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDTO>>> ListarRoles()
        {
            if (!User.EsAdmin()) return Forbid();

            return await _context.Roles.
                Select(r => new RolDTO
                {
                    Numero = r.Numero,
                    Nombre = r.Nombre
                }).ToListAsync();
        }

        [HttpGet("{numero}")]
        public async Task<ActionResult<RolDTO>> BuscarRol(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var r = await _context.Roles.FindAsync(numero);
            if (r == null) return NotFound();

            return Ok(new RolDTO
            {
                Numero = r.Numero,
                Nombre = r.Nombre
            });
        }

        [HttpPost]
        public async Task<ActionResult<RolDTO>> CrearRol([FromBody] RolDTO dto)
        {
            if (!User.EsAdmin()) return Forbid("No tienes permisos para realizar esta acción.");

            var existe = await _context.Roles.AnyAsync(r => r.Numero == dto.Numero);
            if (existe) return BadRequest("Ya existe un rol con el mismo número.");

            var datos = new Rol
            {
                Numero = dto.Numero,
                Nombre = dto.Nombre
            };

            _context.Roles.Add(datos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarRol), new { numero = datos.Numero }, dto);
        }

        [HttpPatch("{numero}")]
        public async Task<IActionResult> ActualizarRol(int numero, [FromBody] ModificarRolDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.Roles.FindAsync(numero);
            if (datos == null) return NotFound();

            if (dto.Nombre != null) datos.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{numero}")]
        public async Task<IActionResult> EliminarRol(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.Roles.FindAsync(numero);
            if (datos == null) return NotFound();

            _context.Roles.Remove(datos);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
