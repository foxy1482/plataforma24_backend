namespace Plataforma24API.Models
{
    public class TareaArchivoDTO
    {
        public int IdTarea { get; set; }

        public int IdArchivo { get; set; }

        // Campos del join
        public string? NombreOriginal { get; set; }
        public string? Ruta { get; set; }
    }

    public class VincTareaArchivoDTO
    {
        public int IdTarea { get; set; }
        public int IdArchivo { get; set; }
    }
}