using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Rol;
using Plataforma24API.Auth;
using Plataforma24API.Models;
using System.Text.RegularExpressions;

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
                    Id = r.Id,
                    Nombre = r.Nombre,
                    PrefijoLegajo = r.PrefijoLegajo
                }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolDTO>> BuscarRol(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var r = await _context.Roles.FindAsync(id);
            if (r == null) return NotFound();

            return Ok(new RolDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                PrefijoLegajo = r.PrefijoLegajo
            });
        }

        [HttpPost]
        public async Task<ActionResult<RolDTO>> CrearRol([FromBody] RolDTO dto)
        {
            if (!User.EsAdmin()) return Forbid("No tienes permisos para realizar esta acción.");

            if (string.IsNullOrWhiteSpace(dto.PrefijoLegajo) || !Regex.IsMatch(dto.PrefijoLegajo, "^[A-Z]{3}$"))
                return BadRequest("El prefijo de legajo debe tener exactamente 3 letras mayúsculas.");

            var existe = await _context.Roles.AnyAsync(r => r.Nombre == dto.Nombre);
            if (existe) return BadRequest("Ya existe un rol con el mismo nombre.");

            var datos = new Rol
            {
                Nombre = dto.Nombre,
                PrefijoLegajo = dto.PrefijoLegajo
            };

            _context.Roles.Add(datos);
            await _context.SaveChangesAsync();

            dto.Id = datos.Id;   
            return CreatedAtAction(nameof(BuscarRol), new { id = datos.Id }, dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarRol(int id, [FromBody] ModificarRolDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.Roles.FindAsync(id);
            if (datos == null) return NotFound();

            if (dto.Nombre != null) datos.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRol(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.Roles.FindAsync(id);
            if (datos == null) return NotFound();

            _context.Roles.Remove(datos);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}