using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("foro_temas")]
    public class ForoTema
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_curso")]
        public int? IdCurso { get; set; }

        [Column("id_autor")]
        public int IdAutor { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = null!;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fijado")]
        public bool Fijado { get; set; }

        [Column("cerrado")]
        public bool Cerrado { get; set; }

        [Column("eliminado")]
        public bool Eliminado { get; set; }

        [Column("id_eliminado_por")]
        public int? IdEliminadoPor { get; set; }

        [Column("fecha_eliminacion")]
        public DateTime? FechaEliminacion { get; set; }
    }
}