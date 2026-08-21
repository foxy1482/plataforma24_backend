using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("materiales")]
    public class Material
    {
        [Key]
        [Column("numero")]
        public int Numero { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("publicada_desde")]
        public DateTime PublicadaDesde { get; set; }

        [Column("num_materia")]
        public int NumMateria { get; set; }
    }
}
