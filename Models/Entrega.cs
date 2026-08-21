using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("entregas")]
    public class Entrega
    {
        [Key]
        [Column("numero")]
        public int Numero { get; set; }

        [Column("num_tarea")]
        public int NumTarea { get; set; }

        [Column("num_legajo")]
        public int NumLegajo { get; set; }

        [Column("fecha_entrega")]
        public DateTime FechaEntrega { get; set; }

        [Column("calificacion")]
        public string? Calificacion { get; set; }
    }
}
