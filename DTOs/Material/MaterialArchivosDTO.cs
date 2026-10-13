namespace Plataforma24API.Models
{
    public class MaterialArchivosDTO
    {
        public int IdMaterial { get; set; }

        public int IdArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincMaterialArchivoDTO
    {
        public int IdMaterial { get; set; }
        public int IdArchivo { get; set; }
    }
}