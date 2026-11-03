using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Boletines
{
    /// <summary>Datos para generar el boletin de un alumno en un periodo.</summary>
    public class GenerarBoletinDTO
    {
        public int IdAlumno { get; set; }

        public int IdPeriodo { get; set; }
    }

    /// <summary>Datos para generar los boletines de todos los alumnos de un curso en un periodo.</summary>
    public class GenerarBoletinesCursoDTO
    {
        public int IdCurso { get; set; }

        public int IdPeriodo { get; set; }
    }

    /// <summary>Cambios sobre un boletin. Los campos nulos no se modifican.</summary>
    public class ModificarBoletinDTO
    {
        public string? Observaciones { get; set; }

        /// <summary>Al pasar a <c>publicado</c> el boletin queda visible para el alumno.</summary>
        public EstadoBoletin? Estado { get; set; }
    }

    /// <summary>Nota de una materia dentro de un boletin. Los campos nulos no se modifican.</summary>
    public class ModificarNotaDTO
    {
        [Range(1, 10)]
        public decimal? Nota { get; set; }

        [StringLength(500)]
        public string? Observacion { get; set; }
    }

    /// <summary>Resumen de un boletin: promedio, notas cargadas e inasistencias del bimestre.</summary>
    public class BoletinResumenDTO
    {
        public int IdBoletin { get; set; }

        public EstadoBoletin Estado { get; set; }

        public int IdAlumno { get; set; }

        public string NumeroLegajo { get; set; } = null!;

        public string? Apellido { get; set; }

        public string? Nombre { get; set; }

        public int IdCurso { get; set; }

        public int CicloLectivo { get; set; }

        public short Bimestre { get; set; }

        public long MateriasDelCurso { get; set; }

        public long MateriasEnBoletin { get; set; }

        public long NotasCargadas { get; set; }

        public decimal? Promedio { get; set; }

        public long MateriasDesaprobadas { get; set; }

        public decimal InasistenciasBimestre { get; set; }

        public DateTime? FechaPublicacion { get; set; }
    }

    /// <summary>Nota de una materia dentro de un boletin.</summary>
    public class BoletinMateriaDTO
    {
        public int IdBoletin { get; set; }

        public EstadoBoletin Estado { get; set; }

        public int CicloLectivo { get; set; }

        public short Bimestre { get; set; }

        public int IdAlumno { get; set; }

        public string NumeroLegajo { get; set; } = null!;

        public string? Apellido { get; set; }

        public string? Nombre { get; set; }

        public int IdCurso { get; set; }

        public string Curso { get; set; } = null!;

        public int IdMateria { get; set; }

        public string Materia { get; set; } = null!;

        public int? IdProfesor { get; set; }

        public string? Profesor { get; set; }

        public decimal? Nota { get; set; }

        public string? Observacion { get; set; }

        public DateTime? FechaCarga { get; set; }
    }

    /// <summary>Boletin completo: resumen mas la nota de cada materia.</summary>
    public class BoletinDetalleDTO
    {
        public BoletinResumenDTO Resumen { get; set; } = null!;

        public List<BoletinMateriaDTO> Notas { get; set; } = new();
    }
}
