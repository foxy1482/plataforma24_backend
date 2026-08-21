using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("usuarios")]
    [Authorize] // Protegemos todos los endpoints de usuario
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET /usuarios/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ListarUsuarios()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    NumeroLegajo = u.NumeroLegajo,
                    Email = u.Email,
                    NombreUsuario = u.NombreUsuario,
                    NumRol = u.NumRol,
                    NumCurso = u.NumCurso
                }).ToListAsync();
        }

        // GET /usuarios/{legajo}
        [HttpGet("{legajo}")]
        public async Task<ActionResult<UsuarioDTO>> ObtenerUsuario(int legajo)
        {
            var u = await _context.Usuarios.FindAsync(legajo);
            if (u == null) return NotFound();

            return new UsuarioDTO
            {
                NumeroLegajo = u.NumeroLegajo,
                Email = u.Email,
                NombreUsuario = u.NombreUsuario,
                NumRol = u.NumRol,
                NumCurso = u.NumCurso
            };
        }

        // GET /usuarios/curso/{idCurso}
        // Retorna alumnos del curso, excluyendo al profesor (Rol 2 asumiendo tu esquema)
        [HttpGet("curso/{numCurso}")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuariosPorCurso(int numCurso)
        {
            return await _context.Usuarios
                .Where(u => u.NumCurso == numCurso)
                .Select(u => new UsuarioDTO
                {
                    NumeroLegajo = u.NumeroLegajo,
                    NombreUsuario = u.NombreUsuario,
                    Email = u.Email,
                    NumRol = u.NumRol
                }).ToListAsync();
        }

        // PATCH /usuarios/{legajo}
        [HttpPatch("{legajo}")]
        public async Task<IActionResult> ActualizarUsuario(int legajo, [FromBody] ModificarUsuarioDTO dto)
        {
            if (!User.TienePermiso(legajo)) return Forbid();

            var usuario = await _context.Usuarios.FindAsync(legajo);
            if (usuario == null) return NotFound();

            if (!User.EsAdmin())
            {
                if (dto.NumRol.HasValue || dto.NumCurso.HasValue)
                {
                    return BadRequest("No tienes permisos para modificar el Rol o el Curso.");
                }
            }

            // Actualización parcial lógica
            if (dto.NombreUsuario != null) usuario.NombreUsuario = dto.NombreUsuario;
            
            if (User.EsAdmin())
            {
                if (dto.NumRol.HasValue && User.EsAdmin()) usuario.NumRol = dto.NumRol.Value;
                if (dto.NumCurso.HasValue && User.EsAdmin()) usuario.NumCurso = dto.NumCurso.Value;

                if (dto.Email != null) usuario.Email = dto.Email;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /usuarios/{legajo}
        [HttpDelete("{legajo}")]
        public async Task<IActionResult> EliminarUsuario(int legajo)
        {
            if (!User.EsAdmin()) return Forbid();

            var usuario = await _context.Usuarios.FindAsync(legajo);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}