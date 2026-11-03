using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.DTOs.Auth
{
    /// <summary>Datos para crear una cuenta propia. El usuario queda registrado con el rol de alumno.</summary>
    public class RegistroDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(3)]
        public required string Nombre { get; set; }

        [Required]
        [MinLength(5)]
        public required string Password { get; set; }
    }
}
