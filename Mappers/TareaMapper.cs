using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Tarea"/> a DTO.</summary>
    public static class TareaMapper
    {
        public static TareaDTO ToDTO(this Tarea tarea)
        {
            return new TareaDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                FechaEntrega = tarea.FechaEntrega,
                PublicadaDesde = tarea.PublicadaDesde,
                IdMateria = tarea.IdMateria
            };
        }
    }
}
