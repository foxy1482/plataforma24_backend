using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("foro_mensajes")]
    public class ForoMensaje
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_tema")]
        public int IdTema { get; set; }

        [Column("id_autor")]
        public int IdAutor { get; set; }

        [Column("id_respuesta_a")]
        public int? IdRespuestaA { get; set; }

        [Column("contenido")]
        public string Contenido { get; set; } = null!;

        [Column("fecha_publicacion")]
        public DateTime FechaPublicacion { get; set; }

        [Column("fecha_edicion")]
        public DateTime? FechaEdicion { get; set; }

        [Column("eliminado")]
        public bool Eliminado { get; set; }

        [Column("id_eliminado_por")]
        public int? IdEliminadoPor { get; set; }

        [Column("fecha_eliminacion")]
        public DateTime? FechaEliminacion { get; set; }
    }
}