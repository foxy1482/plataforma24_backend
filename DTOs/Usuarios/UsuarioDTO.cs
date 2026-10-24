using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Usuarios
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        public string NumeroLegajo { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string NombreUsuario { get; set; } = null!;

        public int IdRol { get; set; }

        public int? IdCurso { get; set; }
    }
}
