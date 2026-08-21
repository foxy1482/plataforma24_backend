using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Materias
{
    public class ModificarMateriaDTO
    {

        public string? Nombre { get; set; }

        public int? NumProfesor { get; set; }

        public int? NumCurso { get; set; }
    }
}
