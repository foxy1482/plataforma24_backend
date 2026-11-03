namespace Plataforma24API.DTOs.Tareas
{
    /// <summary>Tarea asignada a una materia.</summary>
    public class TareaDTO
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = null!;

        public string Descripcion { get; set; } = null!;

        public DateTime FechaEntrega { get; set; }

        public DateTime PublicadaDesde { get; set; }

        public int IdMateria { get; set; }
    }
}
