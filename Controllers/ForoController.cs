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
    [Route("foro")]
    [Authorize]
    public class ForoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ForoController(AppDbContext context)
        {
            _context = context;
        }

        private bool EsModerador()
        {
            return User.EsPersonal();
        }

        private async Task<int?> ObtenerMiCursoAsync()
        {
            int idUsuario = User.GetUserId()!.Value;

            return await _context.Usuarios
                .Where(u => u.Id == idUsuario)
                .Select(u => u.IdCurso)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> PuedeVerTemaAsync(int? idCursoDelTema)
        {
            if (EsModerador() || idCursoDelTema == null) return true;

            var miCurso = await ObtenerMiCursoAsync();
            return miCurso == idCursoDelTema;
        }


        [HttpGet("temas")]
        public async Task<ActionResult<IEnumerable<VForoTema>>> ListarTemas([FromQuery] int? idCurso)
        {
            var cons = _context.VForoTemas.AsQueryable();

            if (idCurso.HasValue)
            {
                cons = cons.Where(t => t.IdCurso == idCurso.Value);
            }

            if (!EsModerador())
            {
                var miCurso = await ObtenerMiCursoAsync();
                cons = cons.Where(t => t.IdCurso == null || t.IdCurso == miCurso);
            }

            var temas = await cons
                .OrderByDescending(t => t.Fijado)
                .ThenByDescending(t => t.UltimoMensaje ?? t.FechaCreacion)
                .ToListAsync();

            return Ok(temas);
        }

        [HttpPost("temas")]
        public async Task<IActionResult> CrearTema([FromBody] CrearTemaDTO dto)
        {
            int idUsuario = User.GetUserId()!.Value;

            var tema = new ForoTema
            {
                IdCurso = dto.IdCurso,
                IdAutor = idUsuario,
                Titulo = dto.Titulo
            };

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.ForoTemas.Add(tema);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(dto.PrimerMensaje))
                {
                    _context.ForoMensajes.Add(new ForoMensaje
                    {
                        IdTema = tema.Id,
                        IdAutor = idUsuario,
                        Contenido = dto.PrimerMensaje
                    });
                    await _context.SaveChangesAsync();
                }

                await tx.CommitAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return CreatedAtAction(nameof(ListarMensajes), new { idTema = tema.Id }, new { id = tema.Id });
        }

        [HttpPatch("temas/{idTema}")]
        public async Task<IActionResult> ModerarTema(int idTema, [FromBody] ModificarTemaDTO dto)
        {
            if (!EsModerador()) return Forbid();

            var tema = await _context.ForoTemas.FindAsync(idTema);
            if (tema == null || tema.Eliminado) return NotFound();

            if (dto.Fijado.HasValue) tema.Fijado = dto.Fijado.Value;
            if (dto.Cerrado.HasValue) tema.Cerrado = dto.Cerrado.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("temas/{idTema}")]
        public async Task<IActionResult> EliminarTema(int idTema)
        {
            var tema = await _context.ForoTemas.FindAsync(idTema);
            if (tema == null) return NotFound();
            if (tema.Eliminado) return NoContent();

            int idUsuario = User.GetUserId()!.Value;
            if (tema.IdAutor != idUsuario && !EsModerador()) return Forbid();

            tema.Eliminado = true;
            tema.IdEliminadoPor = idUsuario;  

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("temas/{idTema}/mensajes")]
        public async Task<ActionResult<IEnumerable<VForoMensaje>>> ListarMensajes(int idTema)
        {
            var tema = await _context.ForoTemas.FindAsync(idTema);
            if (tema == null || (tema.Eliminado && !EsModerador())) return NotFound();

            if (!await PuedeVerTemaAsync(tema.IdCurso)) return Forbid();

            var mensajes = await _context.VForoMensajes
                .Where(m => m.IdTema == idTema)
                .OrderBy(m => m.FechaPublicacion)
                .ToListAsync();

            return Ok(mensajes);
        }

        [HttpPost("temas/{idTema}/mensajes")]
        public async Task<IActionResult> CrearMensaje(int idTema, [FromBody] CrearMensajeDTO dto)
        {
            var tema = await _context.ForoTemas.FindAsync(idTema);
            if (tema == null) return NotFound();

            if (!await PuedeVerTemaAsync(tema.IdCurso)) return Forbid();

            var mensaje = new ForoMensaje
            {
                IdTema = idTema,
                IdAutor = User.GetUserId()!.Value,
                IdRespuestaA = dto.IdRespuestaA,
                Contenido = dto.Contenido
            };

            try
            {
                _context.ForoMensajes.Add(mensaje);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {

                return this.RespuestaDeError(ex);
            }

            return CreatedAtAction(nameof(ListarMensajes), new { idTema }, new { id = mensaje.Id });
        }

        [HttpPatch("mensajes/{idMensaje}")]
        public async Task<IActionResult> EditarMensaje(int idMensaje, [FromBody] ModificarMensajeDTO dto)
        {
            var mensaje = await _context.ForoMensajes.FindAsync(idMensaje);
            if (mensaje == null) return NotFound();

            if (mensaje.IdAutor != User.GetUserId()) return Forbid();

            mensaje.Contenido = dto.Contenido;  

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


        [HttpDelete("mensajes/{idMensaje}")]
        public async Task<IActionResult> EliminarMensaje(int idMensaje)
        {
            var mensaje = await _context.ForoMensajes.FindAsync(idMensaje);
            if (mensaje == null) return NotFound();
            if (mensaje.Eliminado) return NoContent();

            int idUsuario = User.GetUserId()!.Value;
            if (mensaje.IdAutor != idUsuario && !EsModerador()) return Forbid();

            mensaje.Eliminado = true;
            mensaje.IdEliminadoPor = idUsuario;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("mensajes/{idMensaje}/reportes")]
        public async Task<IActionResult> ReportarMensaje(int idMensaje, [FromBody] CrearReporteDTO dto)
        {
            var mensaje = await _context.ForoMensajes.FindAsync(idMensaje);
            if (mensaje == null) return NotFound();

            var tema = await _context.ForoTemas.FindAsync(mensaje.IdTema);
            if (tema == null || !await PuedeVerTemaAsync(tema.IdCurso)) return NotFound();

            var reporte = new ForoReporte
            {
                IdMensaje = idMensaje,
                IdReportante = User.GetUserId()!.Value,
                Motivo = dto.Motivo
            };

            try
            {
                _context.ForoReportes.Add(reporte);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.Buscar(ex)?.SqlState == "23505")
            {

                return Conflict(new { mensaje = "Ya reportaste este mensaje." });
            }

            return Ok(new { id = reporte.Id });
        }

        [HttpGet("reportes")]
        public async Task<ActionResult<IEnumerable<ReporteDTO>>> ListarReportes([FromQuery] bool pendientes = true)
        {
            if (!EsModerador()) return Forbid();

            var cons = _context.ForoReportes.AsQueryable();

            if (pendientes)
            {
                cons = cons.Where(r => !r.Resuelto);
            }

            var reportes = await (from r in cons
                                  join m in _context.ForoMensajes on r.IdMensaje equals m.Id
                                  orderby r.Fecha descending
                                  select new ReporteDTO
                                  {
                                      Id = r.Id,
                                      IdMensaje = r.IdMensaje,
                                      ContenidoMensaje = m.Contenido,
                                      IdAutorMensaje = m.IdAutor,
                                      IdReportante = r.IdReportante,
                                      Motivo = r.Motivo,
                                      Fecha = r.Fecha,
                                      Resuelto = r.Resuelto,
                                      IdResueltoPor = r.IdResueltoPor
                                  }).ToListAsync();

            return Ok(reportes);
        }

        [HttpPatch("reportes/{idReporte}/resolver")]
        public async Task<IActionResult> ResolverReporte(int idReporte)
        {
            if (!EsModerador()) return Forbid();

            var reporte = await _context.ForoReportes.FindAsync(idReporte);
            if (reporte == null) return NotFound();

            reporte.Resuelto = true;
            reporte.IdResueltoPor = User.GetUserId();

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}