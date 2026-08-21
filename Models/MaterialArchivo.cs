using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("material_archivos")]
    public class MaterialArchivo
    {
        [Key]
        [Column("num_material")]
        public int NumMaterial { get; set; }

        [Column("num_archivo")]
        public int NumArchivo { get; set; }
    }
}
