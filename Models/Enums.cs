using NpgsqlTypes;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Plataforma24API.Models
{
    /// <summary>Sexo declarado en los datos personales.</summary>
    public enum Sexo
    {
        [PgName("Masculino")] Masculino,
        [PgName("Femenino")] Femenino
    }
    /// <summary>Turno de cursada.</summary>
    public enum Turno
    {
        manana,
        tarde,
        noche
    }
    /// <summary>Ano de cursada. Los valores se corresponden con el enum <c>years</c> de la base de datos.</summary>
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
    /// <summary>Division dentro de un ano. Los valores se corresponden con el enum <c>divisions</c> de la base de datos.</summary>
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
    /// <summary>Estado de publicacion de un boletin.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EstadoBoletin
    {
        [PgName("borrador")]
        borrador,
        [PgName("publicado")]
        publicado
    }

    /// <summary>Estado de asistencia de un alumno en un dia.</summary>
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

    /// <summary>Forma en que una palabra bloqueada se busca dentro de un texto.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModoCoincidencia
    {
        [PgName("palabra_completa")]
        palabra_completa,
        [PgName("contiene")]
        contiene
    }
}
