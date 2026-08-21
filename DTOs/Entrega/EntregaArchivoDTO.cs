using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.Models
{
    public class EntregaArchivoDTO
    {
        public int NumEntrega { get; set; }

        public int NumArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincEntregaArchivoDTO
    {
        public int NumEntrega { get; set; }
        public int NumArchivo { get; set; }
    }
}
