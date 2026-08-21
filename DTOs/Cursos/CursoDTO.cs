using Plataforma24API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Plataforma24API.DTOs.Cursos
{
    public class CursoDTO
    {
        [Key]
        public int Numero { get; set; }

        public Year Año { get; set; }

        public Division Division { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Turno Turno { get; set; }
        public string Aula { get; set; }

        public int CicloLectivo { get; set; }
    }
}
