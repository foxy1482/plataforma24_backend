using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Materia
{
    public class MateriaDTO
    {
        public int Numero { get; set; }

        public string Nombre { get; set; }

        public int NumProfesor { get; set; }

        public int NumCurso { get; set; }
    }
}
