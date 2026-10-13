using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("materiales")]
    public class Material
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = null!;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("publicada_desde")]
        public DateTime PublicadaDesde { get; set; }

        [Column("id_materia")]
        public int IdMateria { get; set; }
    }
}