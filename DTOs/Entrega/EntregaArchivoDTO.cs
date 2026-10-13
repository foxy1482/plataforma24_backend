namespace Plataforma24API.Models
{
    public class EntregaArchivoDTO
    {
        public int IdEntrega { get; set; }

        public int IdArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincEntregaArchivoDTO
    {
        public int IdEntrega { get; set; }
        public int IdArchivo { get; set; }
    }
}