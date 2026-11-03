namespace Plataforma24API.DTOs.Auth
{
    /// <summary>Datos de la cuenta del usuario autenticado.</summary>
    public class AuthProfileDTO
    {
        public int Id { get; set; }

        public string NumeroLegajo { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public int IdRol { get; set; }

        /// <summary>Nombre del rol, para que el cliente no necesite consultar <c>/roles</c> (solo administradores).</summary>
        public string? Rol { get; set; }

        public int? IdCurso { get; set; }
    }
}
