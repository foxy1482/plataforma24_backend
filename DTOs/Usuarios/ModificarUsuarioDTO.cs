namespace Plataforma24API.DTOs.Usuarios
{
    /// <summary>Cambios sobre un usuario. Solo un administrador puede modificar email, rol y curso.</summary>
    public class ModificarUsuarioDTO
    {
        public string? NombreUsuario { get; set; }

        public string? Email { get; set; }

        public int? IdRol { get; set; }

        public int? IdCurso { get; set; }
    }
}
