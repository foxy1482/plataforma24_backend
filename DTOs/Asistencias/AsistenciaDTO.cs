using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Asistencias
{
    /// <summary>Datos para abrir la planilla de asistencia de un curso en una fecha.</summary>
    public class AbrirAsistenciaDTO
    {
        public int IdCurso { get; set; }

        public DateOnly Fecha { get; set; }
    }

    /// <summary>Cambios sobre una planilla. Los campos nulos no se modifican.</summary>
    public class ModificarAsistenciaDTO
    {
        /// <summary>Una planilla cerrada ya no admite marcas; solo administracion puede reabrirla.</summary>
        public bool? Cerrada { get; set; }

        public string? Observaciones { get; set; }
    }

    /// <summary>Cambio sobre la asistencia de un alumno. Los campos nulos no se modifican.</summary>
    public class ModificarAsistenciaDetalleDTO
    {
        public int IdAlumno { get; set; }

        public EstadoAsistencia? Estado { get; set; }

        [StringLength(300)]
        public string? Observacion { get; set; }
    }

    /// <summary>Cabecera de una planilla de asistencia.</summary>
    public class AsistenciaDTO
    {
        public int Id { get; set; }

        public int IdCurso { get; set; }

        public DateOnly Fecha { get; set; }

        public bool Cerrada { get; set; }

        public int? IdRegistradoPor { get; set; }

        public string? Observaciones { get; set; }
    }

    /// <summary>Asistencia de un alumno dentro de una planilla.</summary>
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

    /// <summary>Planilla completa: cabecera mas el detalle de cada alumno del curso.</summary>
    public class PlanillaAsistenciaDTO : AsistenciaDTO
    {
        public List<AsistenciaDetalleDTO> Detalle { get; set; } = new();
    }

    /// <summary>Inasistencias de un alumno agrupadas por bimestre.</summary>
    public class InasistenciaPeriodoDTO
    {
        public int IdAlumno { get; set; }

        public int IdPeriodo { get; set; }

        public int CicloLectivo { get; set; }

        public short Bimestre { get; set; }

        public long Presentes { get; set; }

        public long Ausentes { get; set; }

        public long Tardes { get; set; }

        public long Justificadas { get; set; }

        /// <summary>Total de inasistencias del bimestre, calculado por la vista <c>v_inasistencias_periodo</c>.</summary>
        public decimal TotalInasistencias { get; set; }

        public long DiasRegistrados { get; set; }
    }

    /// <summary>Inasistencias acumuladas de un alumno en un ciclo lectivo.</summary>
    public class InasistenciaAnualDTO
    {
        public int IdAlumno { get; set; }

        public int CicloLectivo { get; set; }

        public long Ausentes { get; set; }

        public long Tardes { get; set; }

        public long Justificadas { get; set; }

        /// <summary>Total de inasistencias del ciclo, calculado por la vista <c>v_inasistencias_anuales</c>.</summary>
        public decimal TotalInasistencias { get; set; }

        public long DiasRegistrados { get; set; }
    }

    /// <summary>Resumen de asistencia de un alumno, anual y por bimestre.</summary>
    public class ResumenAsistenciaDTO
    {
        public List<InasistenciaAnualDTO> Anuales { get; set; } = new();

        public List<InasistenciaPeriodoDTO> PorBimestre { get; set; } = new();
    }
}
