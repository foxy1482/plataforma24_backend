using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("material_archivos")]
    public class MaterialArchivo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_material")]
        public int IdMaterial { get; set; }

        [Column("id_archivo")]
        public int IdArchivo { get; set; }
    }
}