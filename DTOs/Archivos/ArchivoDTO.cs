namespace Plataforma24API.DTOs.Archivos
{
    /// <summary>Metadatos de un archivo almacenado.</summary>
    public class ArchivoDTO
    {
        public int Id { get; set; }

        public string NombreOriginal { get; set; } = null!;

        public Guid GuidArchivo { get; set; }

        /// <summary>Ruta relativa dentro del almacenamiento.</summary>
        public string Ruta { get; set; } = null!;

        /// <summary>Tipo MIME.</summary>
        public string Tipo { get; set; } = null!;

        /// <summary>Usuario que subio el archivo.</summary>
        public int IdUsuario { get; set; }

        public DateTime FechaSubida { get; set; }
    }
}
