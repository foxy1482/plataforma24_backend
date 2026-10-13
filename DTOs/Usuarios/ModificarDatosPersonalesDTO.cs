using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Usuarios
{
    public class ModificarDatosPersonalesDTO
    {
        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public long? DNI { get; set; }

        public long? TelefonoCelular { get; set; }

        public string? Direccion { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public int? IdFotoArchivo { get; set; }

        public Sexo? Sexo { get; set; }
    }
}
