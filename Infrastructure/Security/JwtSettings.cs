namespace Plataforma24API.Infrastructure.Security
{
    /// <summary>
    /// Parametros de firma y validacion de los tokens JWT, enlazados con la seccion <c>Jwt</c> de la configuracion.
    /// </summary>
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>Largo minimo en bytes que exige HMAC-SHA256 para la clave de firma.</summary>
        private const int LargoMinimoClave = 32;

        public string Key { get; init; } = string.Empty;

        public string Issuer { get; init; } = string.Empty;

        public string Audience { get; init; } = string.Empty;

        /// <summary>Dias de vigencia de un token desde su emision.</summary>
        public int ExpiracionDias { get; init; } = 7;

        public bool EsValida()
        {
            return System.Text.Encoding.UTF8.GetByteCount(Key) >= LargoMinimoClave
                && !string.IsNullOrWhiteSpace(Issuer)
                && !string.IsNullOrWhiteSpace(Audience)
                && ExpiracionDias > 0;
        }
    }
}
