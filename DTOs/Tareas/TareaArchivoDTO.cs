namespace Plataforma24API.DTOs.Tareas
{
    /// <summary>Archivo asociado a una tarea, con los datos necesarios para mostrarlo.</summary>
    public class TareaArchivoDTO
    {
        public int IdTarea { get; set; }

        public int IdArchivo { get; set; }

        public string? NombreOriginal { get; set; }

        public string? Ruta { get; set; }
    }

    /// <summary>Datos para asociar un archivo ya subido a una tarea.</summary>
    public class VincTareaArchivoDTO
    {
        public int IdTarea { get; set; }

        public int IdArchivo { get; set; }
    }
}
