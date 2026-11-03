using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Auth
{
    /// <summary>Credenciales para iniciar sesion.</summary>
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
