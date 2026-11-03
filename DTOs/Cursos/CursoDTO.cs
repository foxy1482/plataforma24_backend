using Plataforma24API.Models;
using System.Text.Json.Serialization;

namespace Plataforma24API.DTOs.Cursos
{
    /// <summary>Curso de la institucion. Al crear un curso, el campo <c>Id</c> se ignora: lo asigna la base de datos.</summary>
    public class CursoDTO
    {
        public int Id { get; set; }

        public Year Año { get; set; }

        public Division Division { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Turno Turno { get; set; }

        public string? Aula { get; set; }

        public int CicloLectivo { get; set; }
    }
}
