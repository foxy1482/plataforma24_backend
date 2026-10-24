using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Usuarios;
using Plataforma24API.Models;
using Plataforma24API.Auth;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("datos")]
    [Authorize]

    public class DatosPersonalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DatosPersonalesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<DatosPersonalesDTO>> ObtenerDatosUsuario(int idUsuario)
        {
            if (!User.TienePermiso(idUsuario)) return Forbid();

            var u = await _context.DatosPersonales.FirstOrDefaultAsync(d => d.IdUsuario == idUsuario);
            if (u == null) return NotFound();

            return Ok(MapearDTO(u));
        }

        [HttpGet("alumnos/curso/{idCurso}")]
        public async Task<ActionResult<IEnumerable<DatosPersonalesDTO>>> ObtenerDatosCurso(int idCurso)
        {
            if (!User.EsAdmin()) return Forbid();

            var lista = await (from u in _context.Usuarios
                               join dp in _context.DatosPersonales on u.Id equals dp.IdUsuario
                               where u.IdCurso == idCurso
                               select dp).ToListAsync();

            return Ok(lista.Select(dp => MapearDTO(dp)));
        }

        [HttpPost]
        public async Task<ActionResult<DatosPersonalesDTO>> CrearDatos([FromBody] DatosPersonalesDTO dto)
        {
            if (!User.TienePermiso(dto.IdUsuario)) return Forbid();

            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Id == dto.IdUsuario);
            if (!usuarioExiste) return BadRequest("El usuario especificado no existe.");

            var existe = await _context.DatosPersonales.AnyAsync(d => d.IdUsuario == dto.IdUsuario);
            if (existe) return BadRequest("Ya existen datos personales para este usuario.");

            var dniRepetido = await _context.DatosPersonales.AnyAsync(d => d.DNI == dto.DNI);
            if (dniRepetido) return BadRequest("Ya existe una persona registrada con ese DNI.");

            var datos = new DatosPersonales
            {
                IdUsuario = dto.IdUsuario,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                TelefonoCelular = dto.TelefonoCelular,
                Direccion = dto.Direccion,
                FechaNacimiento = dto.FechaNacimiento,
                IdFotoArchivo = dto.IdFotoArchivo,
                Sexo = dto.Sexo
            };

            _context.DatosPersonales.Add(datos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerDatosUsuario), new { idUsuario = datos.IdUsuario }, dto);
        }

        [HttpPatch("usuario/{idUsuario}")]
        public async Task<IActionResult> ActualizarDatos(int idUsuario, [FromBody] ModificarDatosPersonalesDTO dto)
        {
            if (!User.TienePermiso(idUsuario)) return Forbid();

            var datos = await _context.DatosPersonales.FirstOrDefaultAsync(d => d.IdUsuario == idUsuario);
            if (datos == null) return NotFound();

            if (dto.Nombre != null) datos.Nombre = dto.Nombre;
            if (dto.Apellido != null) datos.Apellido = dto.Apellido;
            if (dto.DNI != null) datos.DNI = dto.DNI.Value;
            if (dto.TelefonoCelular != null) datos.TelefonoCelular = dto.TelefonoCelular;
            if (dto.Direccion != null) datos.Direccion = dto.Direccion;
            if (dto.FechaNacimiento != default && dto.FechaNacimiento != null) datos.FechaNacimiento = (DateOnly)dto.FechaNacimiento;
            if (dto.IdFotoArchivo.HasValue) datos.IdFotoArchivo = dto.IdFotoArchivo;
            if (dto.Sexo.HasValue) datos.Sexo = dto.Sexo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("usuario/{idUsuario}")]
        public async Task<IActionResult> EliminarDatos(int idUsuario)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.DatosPersonales.FirstOrDefaultAsync(d => d.IdUsuario == idUsuario);
            if (datos == null) return NotFound();

            _context.DatosPersonales.Remove(datos);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static DatosPersonalesDTO MapearDTO(DatosPersonales u)
        {
            return new DatosPersonalesDTO
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                DNI = u.DNI,
                TelefonoCelular = u.TelefonoCelular,
                Direccion = u.Direccion,
                FechaNacimiento = u.FechaNacimiento,
                IdFotoArchivo = u.IdFotoArchivo,
                Sexo = u.Sexo
            };
        }
    }
}