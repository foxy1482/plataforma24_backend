namespace Plataforma24API.DTOs.Entregas
{
    /// <summary>Cambios sobre una entrega. Solo un administrador puede asignar la calificacion.</summary>
    public class ModificarEntregaDTO
    {
        public DateTime? FechaEntrega { get; set; }

        public string? Calificacion { get; set; }
    }
}
