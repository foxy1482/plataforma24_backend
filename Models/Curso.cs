using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Plataforma24API.Models
{
    [Table("cursos")]
    public class Curso
    {
        [Key]
        [Column("numero")]
        public int Numero { get; set; }

        [Column("year")]
        public Year Año { get; set; }

        [Column("division")]
        public Division Division {  get; set; }

        [Column("turno")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Turno Turno { get; set; }

        [Column("aula")]
        public string Aula { get; set; }

        [Column("ciclo_lectivo")]
        public int CicloLectivo { get; set;  }
    }
}
