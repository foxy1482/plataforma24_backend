using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    [Table("materias")]
    public class Materia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("id_profesor")]
        public int? IdProfesor { get; set; }

        [Column("id_curso")]
        public int IdCurso { get; set; }
    }
}