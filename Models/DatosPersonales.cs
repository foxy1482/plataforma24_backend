using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.Models
{
    [Table("datos_personales", Schema = "public")]
    public class DatosPersonales
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("id_usuario")]
        public int IdUsuario {  get; set; }
        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("apellido")]
        public string Apellido { get; set; } = null!;

        [Column("dni")]
        public long DNI { get; set; }

        [Column("telefono_celular")]
        public long? TelefonoCelular { get; set; }

        [Column("direccion")]
        public string Direccion { get; set; } = null!;

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateOnly FechaNacimiento { get; set; }

        [Column("id_foto_archivo")]
        public int? IdFotoArchivo { get; set; }

        [Column("sexo")]
        public Sexo? Sexo { get; set; }
    }
}
