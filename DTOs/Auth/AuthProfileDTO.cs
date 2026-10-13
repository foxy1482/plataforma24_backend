namespace Plataforma24API.DTOs.Auth
{
    public class AuthProfileDTO
    {
        public int Id { get; set; }

        public string NumeroLegajo { get; set; } = null!;   

        public string Email { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public int IdRol { get; set; }

        public int? IdCurso { get; set; }
    }
}
