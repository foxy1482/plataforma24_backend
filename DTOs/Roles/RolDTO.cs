namespace Plataforma24API.DTOs.Roles
{
    /// <summary>Rol de usuario. <c>PrefijoLegajo</c> son tres letras mayusculas con las que comienza el legajo.</summary>
    public class RolDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string? PrefijoLegajo { get; set; }
    }
}
