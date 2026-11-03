namespace Plataforma24API.DTOs.Tareas
{
    /// <summary>Cambios sobre una tarea. Los campos nulos no se modifican.</summary>
    public class ModificarTareaDTO
    {
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }

        public DateTime? FechaEntrega { get; set; }
    }
}
