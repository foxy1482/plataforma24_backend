namespace Plataforma24API.DTOs.Rol
{
    public class RolDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string? PrefijoLegajo { get; set; }
    }
}