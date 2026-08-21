namespace Plataforma24API.DTOs.Usuarios
{
    public class ModificarUsuarioDTO
    {
        public string? NombreUsuario { get; set; }

        public string? Email { get; set;  }

        public int? NumRol { get; set; }
        public int? NumCurso { get; set; }
    }
}
