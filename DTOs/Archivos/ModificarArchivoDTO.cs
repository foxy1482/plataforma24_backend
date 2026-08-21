using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Archivos
{
    public class ModificarArchivoDTO
    {
        public string? NombreOriginal { get; set; }

        public string? Ruta { get; set; }

        public string? Tipo { get; set; }

        public int? NumLegajo { get; set; }

        public DateTime? FechaSubida { get; set; }
    }
}
