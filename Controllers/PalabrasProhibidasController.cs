using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Foro;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("foro/palabras")]
    [Authorize]
    public class PalabrasProhibidasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PalabrasProhibidasController(AppDbContext context)
        {
            _context = context;
        }

        private bool PuedeAdministrar()
        {
            return User.EsAdmin() || User.EsPreceptor();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PalabraDTO>>> ListarPalabras()
        {
            if (!PuedeAdministrar()) return Forbid();

            var palabras = await _context.PalabrasProhibidas
                .OrderBy(p => p.Palabra)
                .ToListAsync();

            return Ok(palabras.Select(p => MapearDTO(p)));
        }


        [HttpPost]
        public async Task<ActionResult<PalabraDTO>> CrearPalabra([FromBody] CrearPalabraDTO dto)
        {
            if (!PuedeAdministrar()) return Forbid();

            var datos = new PalabraProhibida
            {
                Palabra = dto.Palabra.Trim(),
                Modo = dto.Modo,
                IdAgregadaPor = User.GetUserId()
            };

            try
            {
                _context.PalabrasProhibidas.Add(datos);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return CreatedAtAction(nameof(ListarPalabras), null, MapearDTO(datos));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPalabra(int id, [FromBody] ModificarPalabraDTO dto)
        {
            if (!PuedeAdministrar()) return Forbid();

            var palabra = await _context.PalabrasProhibidas.FindAsync(id);
            if (palabra == null) return NotFound();

            if (dto.Activa.HasValue) palabra.Activa = dto.Activa.Value;
            if (dto.Modo.HasValue) palabra.Modo = dto.Modo.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPalabra(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var palabra = await _context.PalabrasProhibidas.FindAsync(id);
            if (palabra == null) return NotFound();

            _context.PalabrasProhibidas.Remove(palabra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarTexto([FromBody] VerificarTextoDTO dto)
        {
            var encontrada = await _context.Database
                .SqlQuery<string>($"SELECT COALESCE(public.fn_buscar_palabra_prohibida({dto.Texto}), '') AS \"Value\"")
                .SingleAsync();

            bool permitido = encontrada == "";

            return Ok(new
            {
                permitido,
                palabra = permitido ? null : encontrada
            });
        }

        private static PalabraDTO MapearDTO(PalabraProhibida p)
        {
            return new PalabraDTO
            {
                Id = p.Id,
                Palabra = p.Palabra,
                Modo = p.Modo,
                Activa = p.Activa,
                IdAgregadaPor = p.IdAgregadaPor,
                FechaAlta = p.FechaAlta
            };
        }
    }
}