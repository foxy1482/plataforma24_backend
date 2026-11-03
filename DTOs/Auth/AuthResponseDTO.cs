namespace Plataforma24API.DTOs.Auth
{
    /// <summary>Respuesta de login y registro.</summary>
    public class AuthResponseDTO
    {
        /// <summary>Token JWT que el cliente debe enviar en el encabezado <c>Authorization: Bearer</c>.</summary>
        public required string Token { get; set; }
    }
}
