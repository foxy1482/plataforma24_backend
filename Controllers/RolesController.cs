using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Roles;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Administracion de roles. Todo el controlador es solo para administradores.</summary>
    [ApiController]
    [Route("roles")]
    [Authorize(Policy = Policies.Admin)]
    public class RolesController : ControllerBase
    {
        private readonly IRolService _service;

        public RolesController(IRolService service)
        {
            _service = service;
        }

        /// <summary>Lista los roles.</summary>
        // GET /roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDTO>>> ListarRoles()
        {
            return Ok(await _service.ListarAsync());
        }

        /// <summary>Devuelve un rol.</summary>
        /// <param name="id">Id del rol.</param>
        /// <response code="404">El rol no existe.</response>
        // GET /roles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDTO>> BuscarRol(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Crea un rol.</summary>
        /// <param name="dto">Nombre y prefijo de legajo (3 letras mayusculas).</param>
        /// <response code="201">Rol creado.</response>
        /// <response code="400">Prefijo invalido o nombre repetido.</response>
        // POST /roles
        [HttpPost]
        public async Task<ActionResult<RolDTO>> CrearRol([FromBody] RolDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarRol), new { id = creado.Id }, creado);
        }

        /// <summary>Cambia el nombre de un rol.</summary>
        /// <param name="id">Id del rol.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /roles/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarRol(int id, [FromBody] ModificarRolDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un rol.</summary>
        /// <param name="id">Id del rol.</param>
        // DELETE /roles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRol(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
