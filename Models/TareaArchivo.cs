using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("tarea_archivos")]
    public class TareaArchivo
    {
        [Key]
        [Column("num_tarea")]
        public int NumTarea { get; set; }

        [Column("num_archivo")]
        public int NumArchivo { get; set; }
    }
}
