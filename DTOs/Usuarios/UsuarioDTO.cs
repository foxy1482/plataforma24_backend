using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Usuarios
{
    public class UsuarioDTO
    {
        [Key]
        [Column("numero_legajo")]
        public int NumeroLegajo { get; set; }

        public string Email { get; set; }

        public string NombreUsuario { get; set; }

        [Column("num_rol")]
        public int NumRol { get; set; }
        [Column("num_curso")]
        public int? NumCurso { get; set; }
    }
}
