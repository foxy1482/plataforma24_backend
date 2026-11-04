using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Plataforma24API.Models
{
    /// <summary>
    /// Cuenta de acceso. El legajo lo genera la base de datos (trigger <c>trg_usuarios_legajo</c>): al crear el
    /// usuario y también al cambiar su rol a uno con otro prefijo.
    /// La aplicación nunca lo escribe; EF lo ignora al guardar.
    /// </summary>
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("numero_legajo")]
        public string NumeroLegajo { get; set; } = null!;
        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = null!;
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;
        [Column("id_rol")]
        public int IdRol { get; set; }
        [Column("id_curso")]
        public int? IdCurso { get; set; }
        [Column("activo")]
        public bool Activo { get; set; } = true;
        [Column("fecha_alta")]
        public DateTime FechaAlta { get; set; }
    }
}
