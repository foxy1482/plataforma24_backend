namespace Plataforma24API.DTOs.Materiales
{
    /// <summary>Archivo asociado a un material didactico, con los datos necesarios para mostrarlo.</summary>
    public class MaterialArchivoDTO
    {
        public int IdMaterial { get; set; }

        public int IdArchivo { get; set; }

        public string? NombreOriginal { get; set; }

        public string? Ruta { get; set; }
    }

    /// <summary>Datos para asociar un archivo ya subido a un material didactico.</summary>
    public class VincMaterialArchivoDTO
    {
        public int IdMaterial { get; set; }

        public int IdArchivo { get; set; }
    }
}
