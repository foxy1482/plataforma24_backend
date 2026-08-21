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

        // GET /datos/usuario/{legajo}
        [HttpGet("usuario/{legajo}")]
        public async Task<ActionResult<DatosPersonalesDTO>> ObtenerDatosUsuario(int legajo)
        {
            if (!User.TienePermiso(legajo)) return Forbid();

            var u = await _context.DatosPersonales.FindAsync(legajo);
            if (u == null) return NotFound();

            return Ok(MapearDTO(u));
        }

        // GET /datos/alumnos/curso/{numero}
        [HttpGet("alumnos/curso/{numero}")]
        public async Task<ActionResult<IEnumerable<DatosPersonalesDTO>>> ObtenerDatosCurso(int numero)
        {
            if (!User.EsAdmin()) return Forbid();

            // Hacemos un Join entre Usuarios y DatosPersonales filtrando por curso
            var lista = await (from u in _context.Usuarios
                               join dp in _context.DatosPersonales on u.NumeroLegajo equals dp.NumeroLegajo
                               where u.NumCurso == numero
                               select dp).ToListAsync();

            return Ok(lista.Select(dp => MapearDTO(dp)));
        }

        // POST /datos/
        [HttpPost]
        public async Task<ActionResult<DatosPersonalesDTO>> CrearDatos([FromBody] DatosPersonalesDTO dto)
        {
            if (!User.TienePermiso(dto.NumeroLegajo)) return Forbid();

            var existe = await _context.DatosPersonales.AnyAsync(d => d.NumeroLegajo == dto.NumeroLegajo);
            if (existe) return BadRequest("Ya existen datos personales para este legajo.");

            var datos = new DatosPersonales
            {
                NumeroLegajo = dto.NumeroLegajo,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                TelefonoCelular = dto.TelefonoCelular,
                Direccion = dto.Direccion,
                FechaNacimiento = dto.FechaNacimiento,
                NumFotoArchivo = dto.NumFotoArchivo,
                Sexo = dto.Sexo
            };

            _context.DatosPersonales.Add(datos);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerDatosUsuario), new { legajo = datos.NumeroLegajo }, dto);
        }

        // PATCH /datos/usuario/{legajo}
        [HttpPatch("usuario/{legajo}")]
        public async Task<IActionResult> ActualizarDatos(int legajo, [FromBody] ModificarDatosPersonalesDTO dto)
        {
            if (!User.TienePermiso(legajo)) return Forbid();

            var datos = await _context.DatosPersonales.FindAsync(legajo);
            if (datos == null) return NotFound();

            // Actualización parcial
            if (dto.Nombre != null) datos.Nombre = dto.Nombre;
            if (dto.Apellido != null) datos.Apellido = dto.Apellido;
            if (dto.DNI != null) datos.DNI = (int) dto.DNI;
            if (dto.TelefonoCelular != null) datos.TelefonoCelular = dto.TelefonoCelular;
            if (dto.Direccion != null) datos.Direccion = dto.Direccion;
            if (dto.FechaNacimiento != default && dto.FechaNacimiento != null) datos.FechaNacimiento = (DateOnly) dto.FechaNacimiento;
            if (dto.NumFotoArchivo.HasValue) datos.NumFotoArchivo = dto.NumFotoArchivo;
            if (dto.Sexo.HasValue) datos.Sexo = dto.Sexo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /datos/usuario/{legajo}
        [HttpDelete("usuario/{legajo}")]
        public async Task<IActionResult> EliminarDatos(int legajo)
        {
            if (!User.EsAdmin()) return Forbid();

            var datos = await _context.DatosPersonales.FindAsync(legajo);
            if (datos == null) return NotFound();

            _context.DatosPersonales.Remove(datos);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Función auxiliar para no repetir código de mapeo
        private static DatosPersonalesDTO MapearDTO(DatosPersonales u)
        {
            return new DatosPersonalesDTO
            {
                NumeroLegajo = u.NumeroLegajo,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                DNI = u.DNI,
                TelefonoCelular = u.TelefonoCelular,
                Direccion = u.Direccion,
                FechaNacimiento = u.FechaNacimiento,
                NumFotoArchivo = u.NumFotoArchivo,
                Sexo = u.Sexo
            };
        }
    }
}