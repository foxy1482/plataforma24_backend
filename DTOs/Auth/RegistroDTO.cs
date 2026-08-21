using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(3)]
    public string Nombre { get; set; }

    [Required]
    [MinLength(5)]
    public string Password { get; set; }
}
