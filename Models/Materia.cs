using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("materias")]
    public class Materia
    {
        [Key]
        [Column("numero")]
        public int Numero { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("num_profesor")]
        public int NumProfesor {  get; set; }

        [Column("num_curso")]
        public int NumCurso { get; set; }
    }
}
