using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Boletin de un alumno para un bimestre. Solo los publicados son visibles para el alumno.</summary>
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

        /// <summary>La completa un trigger de la base de datos al publicar el boletin.</summary>
        [Column("fecha_publicacion")]
        public DateTime? FechaPublicacion { get; set; }
    }
}
