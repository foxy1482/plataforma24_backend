using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("numero_legajo")]
        public int NumeroLegajo { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("num_rol")]
        public int NumRol { get; set; }

        [Column("num_curso")]
        public int? NumCurso { get; set; }
    }
}
