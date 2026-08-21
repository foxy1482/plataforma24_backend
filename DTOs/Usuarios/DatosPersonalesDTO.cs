using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Usuarios
{
    public class DatosPersonalesDTO
    {
        [Key]
        [Column("num_legajo")]
        public int NumeroLegajo { get; set; }
        
        public string Nombre {  get; set; }
        public string Apellido { get; set; }
        
        [Column("dni")]
        public int DNI { get; set; }

        [Column("telefono_celular")]
        public int? TelefonoCelular { get; set; }

        public string Direccion { get; set; }

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateOnly FechaNacimiento { get; set; }

        [Column("num_foto_archivo")]
        public int? NumFotoArchivo { get; set; }
        public Sexo? Sexo { get; set; }
    }
}
