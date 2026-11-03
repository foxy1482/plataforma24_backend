using Plataforma24API.DTOs.Roles;
using Plataforma24API.Models;

namespace Plataforma24API.Mappers
{
    /// <summary>Conversion de <see cref="Rol"/> a DTO.</summary>
    public static class RolMapper
    {
        public static RolDTO ToDTO(this Rol rol)
        {
            return new RolDTO
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                PrefijoLegajo = rol.PrefijoLegajo
            };
        }
    }
}
