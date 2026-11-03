namespace Plataforma24API.DTOs.Foro
{
    /// <summary>Mensaje del foro tal como se lista: incluye autor y reportes pendientes.</summary>
    public class ForoMensajeDTO
    {
        public int Id { get; set; }

        public int IdTema { get; set; }

        public string Tema { get; set; } = null!;

        public int? IdCurso { get; set; }

        public int IdAutor { get; set; }

        public string Autor { get; set; } = null!;

        public string RolAutor { get; set; } = null!;

        /// <summary>Mensaje al que responde, si es una respuesta.</summary>
        public int? IdRespuestaA { get; set; }

        public string Contenido { get; set; } = null!;

        public DateTime FechaPublicacion { get; set; }

        public DateTime? FechaEdicion { get; set; }

        public bool Eliminado { get; set; }

        public long ReportesPendientes { get; set; }
    }
}
