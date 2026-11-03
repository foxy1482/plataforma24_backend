namespace Plataforma24API.DTOs.Entregas
{
    /// <summary>Archivo adjunto a una entrega, con los datos necesarios para mostrarlo.</summary>
    public class EntregaArchivoDTO
    {
        public int IdEntrega { get; set; }

        public int IdArchivo { get; set; }

        public string? NombreOriginal { get; set; }

        public string? Ruta { get; set; }
    }

    /// <summary>Datos para adjuntar un archivo ya subido a una entrega.</summary>
    public class VincEntregaArchivoDTO
    {
        public int IdEntrega { get; set; }

        public int IdArchivo { get; set; }
    }
}
