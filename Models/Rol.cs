using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Plataforma24API.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("prefijo_legajo")]
        public string PrefijoLegajo { get; set; } = null!;
    }
}
