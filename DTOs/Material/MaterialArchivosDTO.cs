using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    public class MaterialArchivosDTO
    {
        public int NumMaterial { get; set; }

        public int NumArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincMaterialArchivoDTO
    {
        public int NumMaterial { get; set; }
        public int NumArchivo { get; set; }
    }
}
