using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("entrega_archivos")]
    public class EntregaArchivo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_entrega")]
        public int IdEntrega { get; set; }

        [Column("id_archivo")]
        public int IdArchivo { get; set; }
    }
}