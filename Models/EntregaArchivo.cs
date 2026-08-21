using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("entrega_archivos")]
    public class EntregaArchivo
    {
        [Key]
        [Column("num_entrega")]
        public int NumEntrega { get; set; }

        [Column("num_archivo")]
        public int NumArchivo { get; set; }
    }
}
