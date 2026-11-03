using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    /// <summary>Relacion entre una tarea y un archivo.</summary>
    [Table("tarea_archivos")]
    public class TareaArchivo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_tarea")]
        public int IdTarea { get; set; }

        [Column("id_archivo")]
        public int IdArchivo { get; set; }
    }
}
