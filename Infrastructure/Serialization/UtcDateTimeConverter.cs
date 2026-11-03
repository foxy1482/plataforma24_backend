using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plataforma24API.Infrastructure.Serialization
{
    /// <summary>
    /// Unifica el manejo de fechas: toda fecha recibida se interpreta como UTC (si llega sin zona se asume UTC,
    /// si llega con offset se convierte) y toda fecha enviada lleva el sufijo "Z". Npgsql solo acepta
    /// <see cref="DateTimeKind.Utc"/> para columnas <c>timestamptz</c>.
    /// </summary>
    public sealed class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return AUtc(reader.GetDateTime());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(AUtc(value));
        }

        private static DateTime AUtc(DateTime valor)
        {
            return valor.Kind switch
            {
                DateTimeKind.Utc => valor,
                DateTimeKind.Local => valor.ToUniversalTime(),
                _ => DateTime.SpecifyKind(valor, DateTimeKind.Utc)
            };
        }
    }
}
