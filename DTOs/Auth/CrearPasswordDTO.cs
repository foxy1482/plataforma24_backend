using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Auth
{
    /// <summary>
    /// Alta de una contrasena nueva para una cuenta blanqueada por un administrador. Sin envio de mail, la identidad
    /// se confirma con el numero de legajo, que el usuario conoce y no se puede deducir del email.
    /// </summary>
    public class CrearPasswordDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        /// <summary>Legajo completo, por ejemplo <c>ALU-0002</c>.</summary>
        [Required]
        public required string NumeroLegajo { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public required string PasswordNueva { get; set; }
    }
}
