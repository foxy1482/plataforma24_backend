using Plataforma24API.DTOs.Materiales;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Material"/> a DTO.</summary>
    public static class MaterialMapper
    {
        public static MaterialDTO ToDTO(this Material material)
        {
            return new MaterialDTO
            {
                Id = material.Id,
                Titulo = material.Titulo,
                Descripcion = material.Descripcion,
                PublicadaDesde = material.PublicadaDesde,
                IdMateria = material.IdMateria
            };
        }
    }
}
