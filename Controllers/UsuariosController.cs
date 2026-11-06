using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Consulta y administracion de usuarios.</summary>
    [ApiController]
    [Route("usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista todos los usuarios, con email y legajo. Solo administradores y preceptores: el registro es
        /// publico y el legajo sirve para crear una contrasena blanqueada.
        /// </summary>
        /// <response code="403">El usuario no es administrador ni preceptor.</response>
        // GET /usuarios
        [HttpGet]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ListarUsuarios()
        {
            return Ok(await _service.ListarAsync());
        }

        /// <summary>Devuelve un usuario.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        /// <response code="404">El usuario no existe.</response>
        // GET /usuarios/{idUsuario}
        [HttpGet("{idUsuario}")]
        public async Task<ActionResult<UsuarioDTO>> ObtenerUsuario(int idUsuario)
        {
            return Ok(await _service.BuscarAsync(idUsuario));
        }

        /// <summary>Lista los usuarios asignados a un curso.</summary>
        /// <param name="numCurso">Id del curso.</param>
        // GET /usuarios/curso/{numCurso}
        [HttpGet("curso/{numCurso}")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> ObtenerUsuariosPorCurso(int numCurso)
        {
            return Ok(await _service.ListarPorCursoAsync(numCurso));
        }

        /// <summary>Modifica un usuario. Solo un administrador puede cambiar email, rol y curso.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        /// <param name="dto">Campos a modificar.</param>
        /// <response code="403">La cuenta es de otro usuario o se intento cambiar el rol o el curso sin ser administrador.</response>
        // PATCH /usuarios/{idUsuario}
        [HttpPatch("{idUsuario}")]
        public async Task<IActionResult> ActualizarUsuario(int idUsuario, [FromBody] ModificarUsuarioDTO dto)
        {
            await _service.ActualizarAsync(idUsuario, dto);
            return NoContent();
        }

        /// <summary>
        /// Blanquea la contrasena de un usuario (olvido, sin envio de mail): en su proximo ingreso crea una nueva
        /// con su email y su numero de legajo. Solo administradores.
        /// </summary>
        /// <param name="idUsuario">Id del usuario.</param>
        /// <response code="204">Contrasena blanqueada.</response>
        /// <response code="400">Es la propia cuenta.</response>
        /// <response code="403">La cuenta es administrativa y quien blanquea no es el Admin general.</response>
        // POST /usuarios/{idUsuario}/blanquear-password
        [HttpPost("{idUsuario}/blanquear-password")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> BlanquearPassword(int idUsuario)
        {
            await _service.BlanquearPasswordAsync(idUsuario);
            return NoContent();
        }

        /// <summary>Elimina un usuario. Solo administradores.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        // DELETE /usuarios/{idUsuario}
        [HttpDelete("{idUsuario}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarUsuario(int idUsuario)
        {
            await _service.EliminarAsync(idUsuario);
            return NoContent();
        }
    }
}
