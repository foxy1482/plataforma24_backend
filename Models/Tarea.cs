using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Tarea asignada a una materia.</summary>
    [Table("tareas")]
    public class Tarea
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = null!;

        [Column("descripcion")]
        public string Descripcion { get; set; } = null!;

        [Column("fecha_entrega", TypeName = "timestamp without time zone")]
        public DateTime FechaEntrega { get; set; }

        [Column("publicada_desde", TypeName = "timestamp without time zone")]
        public DateTime PublicadaDesde { get; set; }

        [Column("id_materia")]
        public int IdMateria { get; set; }
    }
}
