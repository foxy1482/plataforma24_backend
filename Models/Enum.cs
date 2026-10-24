using NpgsqlTypes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Plataforma24API.Models
{
    public enum Sexo
    {
        [PgName("Masculino")] Masculino,
        [PgName("Femenino")] Femenino
    }
    public enum Turno
    {
        manana,
        tarde,
        noche
    }
    public enum Year
    {
        [PgName("1")]
        A1 = 1,
        [PgName("2")]
        A2 = 2,
        [PgName("3")]
        A3 = 3,
        [PgName("4")]
        A4 = 4,
        [PgName("5")]
        A5 = 5,
        [PgName("6")]
        A6 = 6
    }
    public enum Division
    {
        [PgName("1")]
        D1 = 1,
        [PgName("2")]
        D2 = 2,
        [PgName("3")]
        D3 = 3,
        [PgName("4")]
        D4 = 4,
        [PgName("5")]
        D5 = 5,
        [PgName("6")]
        D6 = 6,
        [PgName("7")]
        D7 = 7,
        [PgName("8")]
        D8 = 8,
        [PgName("9")]
        D9 = 9,
        [PgName("10")]
        D10 = 10,
        [PgName("11")]
        D11 = 11,
        [PgName("12")]
        D12 = 12
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstadoBoletin
    {
        [PgName("borrador")]
        borrador,
        [PgName("publicado")]
        publicado
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstadoAsistencia
    {
        [PgName("presente")]
        presente,
        [PgName("ausente")]
        ausente,
        [PgName("tarde")]
        tarde,
        [PgName("justificado")]
        justificado
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModoCoincidencia
    {
        [PgName("palabra_completa")]
        palabra_completa,
        [PgName("contiene")]
        contiene
    }
}