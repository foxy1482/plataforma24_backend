namespace Plataforma24API.DTOs.Foro
{
    /// <summary>Resultado de revisar un texto contra las palabras bloqueadas.</summary>
    public class VerificacionTextoDTO
    {
        public bool Permitido { get; set; }

        /// <summary>Primera palabra bloqueada encontrada; nula si el texto esta permitido.</summary>
        public string? Palabra { get; set; }
    }
}
