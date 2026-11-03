namespace Plataforma24API.DTOs.Archivos
{
    /// <summary>
    /// Archivo recibido para guardar. Desacopla la capa de servicios del tipo <c>IFormFile</c> de ASP.NET:
    /// el controlador traduce la peticion HTTP a este objeto.
    /// </summary>
    public class ArchivoParaSubir
    {
        public Stream Contenido { get; set; } = null!;

        public string NombreOriginal { get; set; } = null!;

        /// <summary>Tipo MIME informado por el cliente.</summary>
        public string Tipo { get; set; } = null!;

        /// <summary>Tamano en bytes.</summary>
        public long Longitud { get; set; }

        /// <summary>Carpeta opcional dentro del almacenamiento.</summary>
        public string? Subcarpeta { get; set; }
    }
}
