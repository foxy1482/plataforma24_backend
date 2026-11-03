namespace Plataforma24API.DTOs.Archivos
{
    /// <summary>Metadatos de un archivo que pueden corregirse. Los campos nulos no se modifican.</summary>
    public class ModificarArchivoDTO
    {
        public string? NombreOriginal { get; set; }

        /// <summary>Nuevo propietario del archivo.</summary>
        public int? IdUsuario { get; set; }
    }
}
