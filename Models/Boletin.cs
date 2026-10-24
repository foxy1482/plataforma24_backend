using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("boletines")]
    public class Boletin
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_alumno")]
        public int IdAlumno { get; set; }

        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Column("id_periodo")]
        public int IdPeriodo { get; set; }

        [Column("estado")]
        public EstadoBoletin Estado { get; set; } = EstadoBoletin.borrador;

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("id_creado_por")]
        public int? IdCreadoPor { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        // La completa el trigger al publicar
        [Column("fecha_publicacion")]
        public DateTime? FechaPublicacion { get; set; }
    }
}