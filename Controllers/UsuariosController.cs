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
            // Primero traemos las entidades y despues mapeamos con el helper
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios.Select(u => MapearDTO(u)));
        }

        // GET /usuarios/{idUsuario}
        [HttpGet("{idUsuario}")]
        public async Task<ActionResult<UsuarioDTO>> ObtenerUsuario(int idUsuario)
        {
            // Ahora "id" es la PK, asi que FindAsync sigue funcionando
            var u = await _context.Usuarios.FindAsync(idUsuario);
            if (u == null) return NotFound();

            return MapearDTO(u);
        }

        // GET /usuarios/curso/{numCurso}
        // Retorna los usuarios del curso
        [HttpGet("curso/{numCurso}")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuariosPorCurso(int numCurso)
        {
            var usuarios = await _context.Usuarios
                .Where(u => u.IdCurso == numCurso)
                .ToListAsync();

            return Ok(usuarios.Select(u => MapearDTO(u)));
        }

        // PATCH /usuarios/{idUsuario}
        [HttpPatch("{idUsuario}")]
        public async Task<IActionResult> ActualizarUsuario(int idUsuario, [FromBody] ModificarUsuarioDTO dto)
        {
            if (!User.TienePermiso(idUsuario)) return Forbid();

            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null) return NotFound();

            if (!User.EsAdmin())
            {
                if (dto.IdRol.HasValue || dto.IdCurso.HasValue)
                {
                    return BadRequest("No tienes permisos para modificar el Rol o el Curso.");
                }
            }

            // Actualización parcial lógica
            if (dto.NombreUsuario != null) usuario.NombreUsuario = dto.NombreUsuario;

            if (User.EsAdmin())
            {
                if (dto.IdRol.HasValue) usuario.IdRol = dto.IdRol.Value;
                if (dto.IdCurso.HasValue) usuario.IdCurso = dto.IdCurso.Value;

                if (dto.Email != null) usuario.Email = dto.Email;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /usuarios/{idUsuario}
        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> EliminarUsuario(int idUsuario)
        {
            if (!User.EsAdmin()) return Forbid();

            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Función auxiliar para no repetir el mapeo en cada endpoint
        private static UsuarioDTO MapearDTO(Usuario u)
        {
            return new UsuarioDTO
            {
                Id = u.Id,
                NumeroLegajo = u.NumeroLegajo,
                Email = u.Email,
                NombreUsuario = u.NombreUsuario,
                IdRol = u.IdRol,
                IdCurso = u.IdCurso
            };
        }
    }
}