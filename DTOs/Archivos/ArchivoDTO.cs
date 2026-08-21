using System.ComponentModel.DataAnnotations.Schema;

namespace Plataforma24API.DTOs.Archivos
{
    public class ArchivoDTO
    {
        public int Numero { get; set; }

        public string NombreOriginal { get; set; }

        public Guid GuidArchivo { get; set; }

        public string Ruta { get; set; }

        public string Tipo { get; set; }

        public int NumLegajo { get; set; }

        public DateTime FechaSubida { get; set; }
    }
}
