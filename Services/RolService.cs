using Plataforma24API.Common.Exceptions;
using Plataforma24API.DTOs.Roles;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace Plataforma24API.Services
{
    /// <summary>Administracion de roles.</summary>
    public class RolService : IRolService
    {
        private static readonly Regex PrefijoValido = new("^[A-Z]{3}$", RegexOptions.Compiled);

        private readonly IRolRepository _roles;
        private readonly IUnitOfWork _unitOfWork;

        public RolService(IRolRepository roles, IUnitOfWork unitOfWork)
        {
            _roles = roles;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RolDTO>> ListarAsync()
        {
            var roles = await _roles.GetAllAsync();
            return roles.Select(r => r.ToDTO());
        }

        /// <inheritdoc />
        public async Task<RolDTO> BuscarAsync(int id)
        {
            var rol = await _roles.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el rol {id}.");

            return rol.ToDTO();
        }

        /// <inheritdoc />
        public async Task<RolDTO> CrearAsync(RolDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PrefijoLegajo) || !PrefijoValido.IsMatch(dto.PrefijoLegajo))
                throw new BadRequestException("El prefijo de legajo debe tener exactamente 3 letras mayúsculas.");

            if (await _roles.ExistePorNombreAsync(dto.Nombre))
                throw new BadRequestException("Ya existe un rol con el mismo nombre.");

            var rol = new Rol
            {
                Nombre = dto.Nombre,
                PrefijoLegajo = dto.PrefijoLegajo
            };

            await _roles.AddAsync(rol);
            await _unitOfWork.SaveChangesAsync();

            return rol.ToDTO();
        }

        /// <inheritdoc />
        public async Task ActualizarAsync(int id, ModificarRolDTO dto)
        {
            var rol = await _roles.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el rol {id}.");

            if (dto.Nombre != null) rol.Nombre = dto.Nombre;

            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task EliminarAsync(int id)
        {
            var rol = await _roles.GetByIdAsync(id)
                ?? throw new NotFoundException($"No se encontró el rol {id}.");

            _roles.Remove(rol);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
