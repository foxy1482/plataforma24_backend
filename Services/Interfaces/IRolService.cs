using Plataforma24API.DTOs.Roles;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de roles.</summary>
    public interface IRolService
    {
        Task<IEnumerable<RolDTO>> ListarAsync();

        /// <exception cref="Common.Exceptions.NotFoundException">El rol no existe.</exception>
        Task<RolDTO> BuscarAsync(int id);

        /// <exception cref="Common.Exceptions.BadRequestException">El prefijo no tiene 3 letras mayusculas o el nombre ya existe.</exception>
        Task<RolDTO> CrearAsync(RolDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El rol no existe.</exception>
        Task ActualizarAsync(int id, ModificarRolDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El rol no existe.</exception>
        Task EliminarAsync(int id);
    }
}
