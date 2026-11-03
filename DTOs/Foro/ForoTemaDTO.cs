namespace Plataforma24API.DTOs.Foro
{
    /// <summary>Tema del foro tal como se lista: incluye autor y actividad.</summary>
    public class ForoTemaDTO
    {
        public int Id { get; set; }

        /// <summary>Curso al que pertenece el tema; nulo si es un tema general visible para todos.</summary>
        public int? IdCurso { get; set; }

        public string Titulo { get; set; } = null!;

        public int IdAutor { get; set; }

        public string Autor { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public bool Fijado { get; set; }

        public bool Cerrado { get; set; }

        public long CantidadMensajes { get; set; }

        public DateTime? UltimoMensaje { get; set; }
    }
}
