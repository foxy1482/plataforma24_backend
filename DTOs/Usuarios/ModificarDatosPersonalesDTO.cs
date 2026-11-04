using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Usuarios
{
    /// <summary>Cambios sobre los datos personales. Los campos nulos no se modifican.</summary>
    public class ModificarDatosPersonalesDTO
    {
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El nombre no puede estar vacío ni superar los 100 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "El apellido no puede estar vacío ni superar los 100 caracteres.")]
        public string? Apellido { get; set; }

        [Range(1_000_000L, 99_999_999L, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos.")]
        public long? DNI { get; set; }

        [Range(1_000_000L, 999_999_999_999_999L, ErrorMessage = "El celular debe tener entre 7 y 15 dígitos.")]
        public long? TelefonoCelular { get; set; }

        [StringLength(200, MinimumLength = 1, ErrorMessage = "El domicilio no puede estar vacío ni superar los 200 caracteres.")]
        public string? Direccion { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public int? IdFotoArchivo { get; set; }

        public Sexo? Sexo { get; set; }
    }
}
