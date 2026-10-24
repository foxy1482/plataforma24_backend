using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Foro
{
    public class CrearTemaDTO
    {
        public int? IdCurso { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Titulo { get; set; } = null!;

        [StringLength(2000)]
        public string? PrimerMensaje { get; set; }
    }

    public class ModificarTemaDTO
    {
        public bool? Fijado { get; set; }

        public bool? Cerrado { get; set; }
    }

    public class CrearMensajeDTO
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Contenido { get; set; } = null!;

        public int? IdRespuestaA { get; set; }
    }

    public class ModificarMensajeDTO
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Contenido { get; set; } = null!;
    }

    public class CrearReporteDTO
    {
        [Required]
        [StringLength(300, MinimumLength = 3)]
        public string Motivo { get; set; } = null!;
    }
    public class ReporteDTO
    {
        public int Id { get; set; }

        public int IdMensaje { get; set; }

        public string ContenidoMensaje { get; set; } = null!;

        public int IdAutorMensaje { get; set; }

        public int IdReportante { get; set; }

        public string Motivo { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public bool Resuelto { get; set; }

        public int? IdResueltoPor { get; set; }
    }

    public class PalabraDTO
    {
        public int Id { get; set; }

        public string Palabra { get; set; } = null!;

        public ModoCoincidencia Modo { get; set; }

        public bool Activa { get; set; }

        public int? IdAgregadaPor { get; set; }

        public DateTime FechaAlta { get; set; }
    }

    public class CrearPalabraDTO
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Palabra { get; set; } = null!;

        public ModoCoincidencia Modo { get; set; } = ModoCoincidencia.palabra_completa;
    }

    public class ModificarPalabraDTO
    {
        public bool? Activa { get; set; }

        public ModoCoincidencia? Modo { get; set; }
    }

    public class VerificarTextoDTO
    {
        [Required]
        public string Texto { get; set; } = null!;
    }
}