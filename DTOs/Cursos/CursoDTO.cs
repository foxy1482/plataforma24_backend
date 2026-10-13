using Plataforma24API.Models;
using System.Text.Json.Serialization;

namespace Plataforma24API.DTOs.Cursos
{
    public class CursoDTO
    {
        public int Id { get; set; }   // solo de salida: al crear, el controller lo ignora

        public Year Año { get; set; }

        public Division Division { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Turno Turno { get; set; }

        public string? Aula { get; set; }

        public int CicloLectivo { get; set; }
    }
}