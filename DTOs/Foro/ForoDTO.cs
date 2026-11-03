using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Foro
{
    /// <summary>Datos para abrir un tema nuevo.</summary>
    public class CrearTemaDTO
    {
        /// <summary>Curso del tema; nulo para un tema general.</summary>
        public int? IdCurso { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Titulo { get; set; } = null!;

        /// <summary>Texto inicial opcional; se publica como primer mensaje del tema.</summary>
        [StringLength(2000)]
        public string? PrimerMensaje { get; set; }
    }

    /// <summary>Acciones de moderacion sobre un tema. Los campos nulos no se modifican.</summary>
    public class ModificarTemaDTO
    {
        public bool? Fijado { get; set; }

        public bool? Cerrado { get; set; }
    }

    /// <summary>Datos para publicar un mensaje en un tema.</summary>
    public class CrearMensajeDTO
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Contenido { get; set; } = null!;

        /// <summary>Mensaje al que se responde, si corresponde.</summary>
        public int? IdRespuestaA { get; set; }
    }

    /// <summary>Nuevo contenido de un mensaje propio.</summary>
    public class ModificarMensajeDTO
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Contenido { get; set; } = null!;
    }

    /// <summary>Datos para reportar un mensaje.</summary>
    public class CrearReporteDTO
    {
        [Required]
        [StringLength(300, MinimumLength = 3)]
        public string Motivo { get; set; } = null!;
    }

    /// <summary>Reporte junto con el mensaje reportado, para que moderacion lo revise.</summary>
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

    /// <summary>Palabra bloqueada en el foro.</summary>
    public class PalabraDTO
    {
        public int Id { get; set; }

        public string Palabra { get; set; } = null!;

        public ModoCoincidencia Modo { get; set; }

        public bool Activa { get; set; }

        public int? IdAgregadaPor { get; set; }

        public DateTime FechaAlta { get; set; }
    }

    /// <summary>Datos para bloquear una palabra nueva.</summary>
    public class CrearPalabraDTO
    {
        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Palabra { get; set; } = null!;

        public ModoCoincidencia Modo { get; set; } = ModoCoincidencia.palabra_completa;
    }

    /// <summary>Cambios sobre una palabra bloqueada. Los campos nulos no se modifican.</summary>
    public class ModificarPalabraDTO
    {
        public bool? Activa { get; set; }

        public ModoCoincidencia? Modo { get; set; }
    }

    /// <summary>Texto a revisar contra la lista de palabras bloqueadas.</summary>
    public class VerificarTextoDTO
    {
        [Required]
        public string Texto { get; set; } = null!;
    }
}
