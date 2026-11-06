using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Auth
{
    /// <summary>Cambio de contrasena del usuario autenticado.</summary>
    public class CambiarPasswordDTO
    {
        /// <summary>Contrasena vigente. Se ignora si la cuenta tiene la contrasena blanqueada.</summary>
        public string? PasswordActual { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public required string PasswordNueva { get; set; }
    }
}
