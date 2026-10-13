using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Usuarios
{
    public class DatosPersonalesDTO
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public long DNI { get; set; }

        public long? TelefonoCelular { get; set; }

        public string Direccion { get; set; } = null!;

        public DateOnly FechaNacimiento { get; set; }

        public int? IdFotoArchivo { get; set; }

        public Sexo? Sexo { get; set; }
    }
}
