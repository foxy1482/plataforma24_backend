using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("asistencias")]
    public class Asistencia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_curso")]
        public int IdCurso { get; set; }

        [Column("fecha", TypeName = "date")]
        public DateOnly Fecha { get; set; }

        [Column("id_registrado_por")]
        public int? IdRegistradoPor { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }

        [Column("cerrada")]
        public bool Cerrada { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }
}