namespace Plataforma24API.DTOs.Tareas
{
    /// <summary>Datos para publicar una tarea.</summary>
    public class CrearTareaDTO
    {
        public string Titulo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaEntrega { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int IdMateria { get; set; }
    }
}
