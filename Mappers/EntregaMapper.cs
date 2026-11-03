using Plataforma24API.DTOs.Entregas;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Entrega"/> a DTO.</summary>
    public static class EntregaMapper
    {
        public static EntregaRespuestaDTO ToDTO(this Entrega entrega)
        {
            return new EntregaRespuestaDTO
            {
                Id = entrega.Id,
                IdTarea = entrega.IdTarea,
                IdUsuario = entrega.IdUsuario,
                FechaEntrega = entrega.FechaEntrega,
                Calificacion = entrega.Calificacion
            };
        }
    }
}
