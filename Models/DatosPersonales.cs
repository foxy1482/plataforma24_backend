using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.Models
{
    [Table("datos_personales", Schema = "public")]
    public class DatosPersonales
    {
        [Key]
        [Column("num_legajo")]
        public int NumeroLegajo { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("dni")]
        public int DNI { get; set; }

        [Column("telefono_celular")]
        public int? TelefonoCelular { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; }

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateOnly FechaNacimiento { get; set; }

        [Column("num_foto_archivo")]
        public int? NumFotoArchivo { get; set; }

        [Column("sexo")]
        public Sexo? Sexo { get; set; }
    }
}
