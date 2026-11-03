using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Entrega de una tarea realizada por un alumno.</summary>
    [Table("entregas")]
    public class Entrega
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_tarea")]
        public int IdTarea { get; set; }

        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("fecha_entrega")]
        public DateTime FechaEntrega { get; set; }

        [Column("calificacion")]
        public string? Calificacion { get; set; }
    }
}
