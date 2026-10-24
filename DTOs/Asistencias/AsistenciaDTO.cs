using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Asistencias
{
    public class AbrirAsistenciaDTO
    {
        public int IdCurso { get; set; }

        public DateOnly Fecha { get; set; }
    }

    public class ModificarAsistenciaDTO
    {
        public bool? Cerrada { get; set; }

        public string? Observaciones { get; set; }
    }

    public class ModificarAsistenciaDetalleDTO
    {
        public int IdAlumno { get; set; }

        public EstadoAsistencia? Estado { get; set; }

        [StringLength(300)]
        public string? Observacion { get; set; }
    }

    public class AsistenciaDTO
    {
        public int Id { get; set; }

        public int IdCurso { get; set; }

        public DateOnly Fecha { get; set; }

        public bool Cerrada { get; set; }

        public int? IdRegistradoPor { get; set; }

        public string? Observaciones { get; set; }
    }

    public class AsistenciaDetalleDTO
    {
        public int Id { get; set; }

        public int IdAlumno { get; set; }

        public string? NumeroLegajo { get; set; }

        public string? Apellido { get; set; }

        public string? Nombre { get; set; }

        public EstadoAsistencia Estado { get; set; }

        public string? Observacion { get; set; }
    }

    public class PlanillaAsistenciaDTO : AsistenciaDTO
    {
        public List<AsistenciaDetalleDTO> Detalle { get; set; } = new();
    }
    public class ResumenAsistenciaDTO
    {
        public List<VInasistenciaAnual> Anuales { get; set; } = new();

        public List<VInasistenciaPeriodo> PorBimestre { get; set; } = new();
    }
}