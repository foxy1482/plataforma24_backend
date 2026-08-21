using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    public class TareaArchivoDTO
    {
        public int NumTarea { get; set; }

        public int NumArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincTareaArchivoDTO
    {
        public int NumTarea { get; set; }
        public int NumArchivo { get; set; }
    }
}
