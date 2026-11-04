using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Usuarios
{
    /// <summary>Datos personales de un usuario.</summary>
    public class DatosPersonalesDTO
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
        public string Apellido { get; set; } = null!;

        [Range(1_000_000L, 99_999_999L, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos.")]
        public long DNI { get; set; }

        [Range(1_000_000L, 999_999_999_999_999L, ErrorMessage = "El celular debe tener entre 7 y 15 dígitos.")]
        public long? TelefonoCelular { get; set; }

        [Required(ErrorMessage = "El domicilio es obligatorio.")]
        [StringLength(200, ErrorMessage = "El domicilio no puede superar los 200 caracteres.")]
        public string Direccion { get; set; } = null!;

        public DateOnly FechaNacimiento { get; set; }

        public int? IdFotoArchivo { get; set; }

        public Sexo? Sexo { get; set; }
    }
}
