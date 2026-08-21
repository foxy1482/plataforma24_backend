using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [Column("numero")]
        public int Numero {  get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }
    }
}
