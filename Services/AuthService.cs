using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Auth;
using Plataforma24API.Mappers;
using Plataforma24API.Models;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Services
{
    /// <summary>Inicio de sesion, registro y consulta del perfil. Las credenciales invalidas se informan con un mensaje unico para no revelar si el email existe.</summary>
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUsuarioRepository usuarios,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _usuarios = usuarios;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        /// <inheritdoc />
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var usuario = await _usuarios.GetByEmailAsync(dto.Email);

            if (usuario == null || !_passwordHasher.Verify(dto.Password, usuario.PasswordHash))
                throw new UnauthorizedException("Email o contraseña incorrectos.");

            if (!usuario.Activo)
                throw new UnauthorizedException("El usuario está inactivo.");

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.Id, usuario.IdRol) };
        }

        /// <inheritdoc />
        public async Task<AuthResponseDTO> RegistroAsync(RegistroDTO dto)
        {
            if (await _usuarios.ExistePorEmailAsync(dto.Email))
                throw new BadRequestException("El email ya está registrado.");

            if (await _usuarios.ExistePorNombreUsuarioAsync(dto.Nombre))
                throw new BadRequestException("El nombre de usuario ya está en uso.");

            var usuario = new Usuario
            {
                Email = dto.Email,
                NombreUsuario = dto.Nombre,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                IdRol = RolIds.Alumno
            };

            await _usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.Id, usuario.IdRol) };
        }

        /// <inheritdoc />
        public async Task<AuthProfileDTO> ObtenerPerfilAsync(int idUsuario)
        {
            var usuario = await _usuarios.GetByIdAsync(idUsuario)
                ?? throw new NotFoundException("Usuario no encontrado.");

            return usuario.ToProfileDTO();
        }
    }
}
