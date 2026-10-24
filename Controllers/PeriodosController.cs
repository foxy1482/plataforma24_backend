using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plataforma24API.Auth;
using Plataforma24API.Data;
using Plataforma24API.DTOs.Periodos;
using Plataforma24API.Models;

namespace Plataforma24API.Controllers
{
    [ApiController]
    [Route("periodos")]
    [Authorize]
    public class PeriodosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PeriodosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoDTO>>> ListarPeriodos([FromQuery] int? ciclo)
        {
            var cons = _context.PeriodosBimestrales.AsQueryable();

            if (ciclo.HasValue)
            {
                cons = cons.Where(p => p.CicloLectivo == ciclo.Value);
            }

            var resultados = await cons
                .OrderBy(p => p.CicloLectivo)
                .ThenBy(p => p.Bimestre)
                .ToListAsync();

            return Ok(resultados.Select(p => MapearDTO(p)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeriodoDTO>> BuscarPeriodo(int id)
        {
            var p = await _context.PeriodosBimestrales.FindAsync(id);
            if (p == null) return NotFound();

            return Ok(MapearDTO(p));
        }

        [HttpPost]
        public async Task<ActionResult<PeriodoDTO>> CrearPeriodo([FromBody] PeriodoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            if (dto.Bimestre < 1 || dto.Bimestre > 4)
                return BadRequest("El bimestre debe ser un número entre 1 y 4.");

            if (dto.FechaFin <= dto.FechaInicio)
                return BadRequest("La fecha de fin debe ser posterior a la de inicio.");

            var existe = await _context.PeriodosBimestrales
                .AnyAsync(p => p.CicloLectivo == dto.CicloLectivo && p.Bimestre == dto.Bimestre);
            if (existe) return BadRequest("Ya existe ese bimestre para el ciclo lectivo.");

            var datos = new PeriodoBimestral
            {
                CicloLectivo = dto.CicloLectivo,
                Bimestre = dto.Bimestre,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin
            };

            try
            {
                _context.PeriodosBimestrales.Add(datos);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            dto.Id = datos.Id;  
            return CreatedAtAction(nameof(BuscarPeriodo), new { id = datos.Id }, dto);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPeriodo(int id, [FromBody] ModificarPeriodoDTO dto)
        {
            if (!User.EsAdmin()) return Forbid();

            var periodo = await _context.PeriodosBimestrales.FindAsync(id);
            if (periodo == null) return NotFound($"No se encontró el período {id}");

            if (dto.FechaInicio.HasValue) periodo.FechaInicio = dto.FechaInicio.Value;
            if (dto.FechaFin.HasValue) periodo.FechaFin = dto.FechaFin.Value;

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
        public async Task<IActionResult> EliminarPeriodo(int id)
        {
            if (!User.EsAdmin()) return Forbid();

            var periodo = await _context.PeriodosBimestrales.FindAsync(id);
            if (periodo == null) return NotFound();

            try
            {
                _context.PeriodosBimestrales.Remove(periodo);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (PostgresErrores.EsErrorDeNegocio(ex))
            {
                return this.RespuestaDeError(ex);
            }

            return NoContent();
        }

        private static PeriodoDTO MapearDTO(PeriodoBimestral p)
        {
            return new PeriodoDTO
            {
                Id = p.Id,
                CicloLectivo = p.CicloLectivo,
                Bimestre = p.Bimestre,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin
            };
        }
    }
}