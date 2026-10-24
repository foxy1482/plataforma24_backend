using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Boletines
{

    public class GenerarBoletinDTO
    {
        public int IdAlumno { get; set; }

        public int IdPeriodo { get; set; }
    }

    public class GenerarBoletinesCursoDTO
    {
        public int IdCurso { get; set; }

        public int IdPeriodo { get; set; }
    }

    public class ModificarBoletinDTO
    {
        public string? Observaciones { get; set; }


        public EstadoBoletin? Estado { get; set; }
    }

    public class ModificarNotaDTO
    {
        [Range(1, 10)]
        public decimal? Nota { get; set; }

        [StringLength(500)]
        public string? Observacion { get; set; }
    }
    public class BoletinDetalleDTO
    {
        public VBoletinResumen Resumen { get; set; } = null!;

        public List<VBoletinDetalle> Notas { get; set; } = new();
    }
}