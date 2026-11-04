using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>
    /// Datos personales de los usuarios. Cada usuario accede a los suyos; el personal consulta los de todos;
    /// los administradores y preceptores los cargan, modifican y eliminan.
    /// </summary>
    [ApiController]
    [Route("datos")]
    [Authorize]
    public class DatosPersonalesController : ControllerBase
    {
        private readonly IDatosPersonalesService _service;

        public DatosPersonalesController(IDatosPersonalesService service)
        {
            _service = service;
        }

        /// <summary>Devuelve los datos personales de un usuario.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        /// <response code="403">Los datos pertenecen a otro usuario.</response>
        /// <response code="404">El usuario no cargo sus datos.</response>
        // GET /datos/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<DatosPersonalesDTO>> ObtenerDatosUsuario(int idUsuario)
        {
            return Ok(await _service.ObtenerPorUsuarioAsync(idUsuario));
        }

        /// <summary>Lista los datos personales de los usuarios de un curso. Solo personal.</summary>
        /// <param name="idCurso">Id del curso.</param>
        // GET /datos/alumnos/curso/{idCurso}
        [HttpGet("alumnos/curso/{idCurso}")]
        [Authorize(Policy = Policies.Personal)]
        public async Task<ActionResult<IEnumerable<DatosPersonalesDTO>>> ObtenerDatosCurso(int idCurso)
        {
            return Ok(await _service.ListarPorCursoAsync(idCurso));
        }

        /// <summary>Carga los datos personales de un usuario.</summary>
        /// <param name="dto">Datos personales; <c>IdUsuario</c> indica a quien pertenecen.</param>
        /// <response code="201">Datos cargados.</response>
        /// <response code="400">El usuario no existe, ya tiene datos o el DNI esta registrado.</response>
        // POST /datos
        [HttpPost]
        public async Task<ActionResult<DatosPersonalesDTO>> CrearDatos([FromBody] DatosPersonalesDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerDatosUsuario), new { idUsuario = creado.IdUsuario }, creado);
        }

        /// <summary>Modifica los datos personales de un usuario.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /datos/usuario/{idUsuario}
        [HttpPatch("usuario/{idUsuario}")]
        public async Task<IActionResult> ActualizarDatos(int idUsuario, [FromBody] ModificarDatosPersonalesDTO dto)
        {
            await _service.ActualizarAsync(idUsuario, dto);
            return NoContent();
        }

        /// <summary>Elimina los datos personales de un usuario. Solo administradores y preceptores.</summary>
        /// <param name="idUsuario">Id del usuario.</param>
        // DELETE /datos/usuario/{idUsuario}
        [HttpDelete("usuario/{idUsuario}")]
        [Authorize(Policy = Policies.AdminOPreceptor)]
        public async Task<IActionResult> EliminarDatos(int idUsuario)
        {
            await _service.EliminarAsync(idUsuario);
            return NoContent();
        }
    }
}
