namespace Plataforma24API.DTOs.Archivos
{
    public class ArchivoDTO
    {
        public int Id { get; set; }

        public string NombreOriginal { get; set; } = null!;

        public Guid GuidArchivo { get; set; }

        public string Ruta { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public int IdUsuario { get; set; }

        public DateTime FechaSubida { get; set; }
    }
}