namespace Plataforma24API.DTOs.Auth
{
    public class AuthProfileDTO
    {
        public int NumeroLegajo { get; set; }

        public string Email { get; set;  }

        public string Nombre { get; set; }

        public int NumRol { get; set; }

        public int? NumCurso { get; set; }
    }
}
