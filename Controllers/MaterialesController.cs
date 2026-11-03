using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Materiales;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Consulta y administracion de materiales didacticos.</summary>
    [ApiController]
    [Route("materiales")]
    [Authorize]
    public class MaterialesController : ControllerBase
    {
        private readonly IMaterialService _service;

        public MaterialesController(IMaterialService service)
        {
            _service = service;
        }

        /// <summary>Lista los materiales, opcionalmente de una materia.</summary>
        /// <param name="idMateria">Id de la materia.</param>
        // GET /materiales?idMateria={id}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaterialDTO>>> ListarMateriales([FromQuery] int? idMateria)
        {
            return Ok(await _service.ListarAsync(idMateria));
        }

        /// <summary>Devuelve un material.</summary>
        /// <param name="id">Id del material.</param>
        /// <response code="404">El material no existe.</response>
        // GET /materiales/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialDTO>> BuscarMaterial(int id)
        {
            return Ok(await _service.BuscarAsync(id));
        }

        /// <summary>Publica un material. Solo administradores.</summary>
        /// <param name="dto">Datos del material.</param>
        /// <response code="201">Material creado.</response>
        // POST /materiales
        [HttpPost]
        [Authorize(Policy = Policies.Admin)]
        public async Task<ActionResult<MaterialDTO>> CrearMaterial([FromBody] CrearMaterialDTO dto)
        {
            var creado = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarMaterial), new { id = creado.Id }, creado);
        }

        /// <summary>Modifica un material. Solo administradores.</summary>
        /// <param name="id">Id del material.</param>
        /// <param name="dto">Campos a modificar.</param>
        // PATCH /materiales/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> ActualizarMaterial(int id, [FromBody] ModificarMaterialDTO dto)
        {
            await _service.ActualizarAsync(id, dto);
            return NoContent();
        }

        /// <summary>Elimina un material. Solo administradores.</summary>
        /// <param name="id">Id del material.</param>
        /// <response code="400">El material todavia tiene datos asociados.</response>
        // DELETE /materiales/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EliminarMaterial(int id)
        {
            await _service.EliminarAsync(id);
            return NoContent();
        }
    }
}
