namespace Plataforma24API.DTOs.Entregas
{
    /// <summary>Entrega de una tarea realizada por un alumno.</summary>
    public class EntregaRespuestaDTO
    {
        public int Id { get; set; }

        public int IdTarea { get; set; }

        public int IdUsuario { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string? Calificacion { get; set; }
    }
}
