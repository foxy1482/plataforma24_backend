using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Boletines;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("boletines")]
    [Authorize]
    public class BoletinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BoletinesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("alumno/{idAlumno}")]
        public async Task<ActionResult<IEnumerable<VBoletinResumen>>> ListarPorAlumno(
            int idAlumno,
            [FromQuery] int? ciclo)
        {
            bool esPersonal = User.EsPersonal();
            if (!esPersonal && User.GetUserId() != idAlumno) return Forbid();

            var cons = _context.VBoletinResumenes.Where(b => b.IdAlumno == idAlumno);

            if (ciclo.HasValue)
            {
                cons = cons.Where(b => b.CicloLectivo == ciclo.Value);
            }

            if (!esPersonal)
            {
                cons = cons.Where(b => b.Estado == EstadoBoletin.publicado);
            }

            var resultados = await cons
                .OrderBy(b => b.CicloLectivo)
                .ThenBy(b => b.Bimestre)
                .ToListAsync();

            return Ok(resultados);
        }

        [HttpGet("curso/{idCurso}")]
        public async Task<ActionResult<IEnumerable<VBoletinResumen>>> ListarPorCurso(
            int idCurso,
            [FromQuery] int? ciclo,
            [FromQuery] short? bimestre)
        {
            if (!User.EsPersonal()) return Forbid();

            var cons = _context.VBoletinResumenes.Where(b => b.IdCurso == idCurso);

            if (ciclo.HasValue) cons = cons.Where(b => b.CicloLectivo == ciclo.Value);
            if (bimestre.HasValue) cons = cons.Where(b => b.Bimestre == bimestre.Value);

            var resultados = await cons
                .OrderBy(b => b.Apellido)
                .ThenBy(b => b.Nombre)
                .ToListAsync();

            return Ok(resultados);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BoletinDetalleDTO>> BuscarBoletin(int id)
        {
            var resumen = await _context.VBoletinResumenes.FirstOrDefaultAsync(b => b.IdBoletin == id);
            if (resumen == null) return NotFound();

            if (!User.EsPersonal())
            {
                if (resumen.IdAlumno != User.GetUserId()) return Forbid();

                if (resumen.Estado != EstadoBoletin.publicado) return NotFound();
            }

            var notas = await _context.VBoletinDetalles
                .Where(d => d.IdBoletin == id)
                .OrderBy(d => d.Materia)
                .ToListAsync();

            return Ok(new BoletinDetalleDTO { Resumen = resumen, Notas = notas });
        }

        [HttpPost("generar")]
        public async Task<IActionResult> GenerarBoletin([FromBody] GenerarBoletinDTO dto)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            int idCreador = User.GetUserId()!.Value;

            try
            {

                var idBoletin = await _context.Database
                    .SqlQuery<int>($"SELECT public.fn_generar_boletin({dto.IdAlumno}, {dto.IdPeriodo}, {idCreador}) AS \"Value\"")
                    .SingleAsync();

                return Ok(new { idBoletin });
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {

                return this.RespuestaDeError(ex);
            }
        }

        [HttpPost("generar-curso")]
        public async Task<IActionResult> GenerarBoletinesCurso([FromBody] GenerarBoletinesCursoDTO dto)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            int idCreador = User.GetUserId()!.Value;

            try
            {
                var alumnos = await _context.Database
                    .SqlQuery<int>($"SELECT public.fn_generar_boletines_curso({dto.IdCurso}, {dto.IdPeriodo}, {idCreador}) AS \"Value\"")
                    .SingleAsync();

                return Ok(new { alumnos });
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }
        }

        [HttpPatch("{idBoletin}/notas/{idMateria}")]
        public async Task<IActionResult> CargarNota(int idBoletin, int idMateria, [FromBody] ModificarNotaDTO dto)
        {
            if (!User.EsPersonal()) return Forbid();

            var nota = await _context.BoletinNotas
                .FirstOrDefaultAsync(n => n.IdBoletin == idBoletin && n.IdMateria == idMateria);
            if (nota == null) return NotFound("Ese boletín no tiene esa materia.");

            if (dto.Nota.HasValue) nota.Nota = dto.Nota.Value;
            if (dto.Observacion != null) nota.Observacion = dto.Observacion;

            nota.IdCargadoPor = User.GetUserId();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarBoletin(int id, [FromBody] ModificarBoletinDTO dto)
        {
            if (!User.EsAdmin() && !User.EsPreceptor()) return Forbid();

            var boletin = await _context.Boletines.FindAsync(id);
            if (boletin == null) return NotFound($"No se encontró el boletín {id}");

            if (dto.Observaciones != null) boletin.Observaciones = dto.Observaciones;
            if (dto.Estado.HasValue) boletin.Estado = dto.Estado.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {

                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarBoletin(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var boletin = await _context.Boletines.FindAsync(id);
            if (boletin == null) return NotFound();

            try
            {
                _context.Boletines.Remove(boletin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }
    }
}