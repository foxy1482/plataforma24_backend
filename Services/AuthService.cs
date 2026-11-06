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
        private readonly IRolRepository _roles;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUsuarioRepository usuarios,
            IRolRepository roles,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _usuarios = usuarios;
            _roles = roles;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        /// <inheritdoc />
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var usuario = await _usuarios.GetByEmailAsync(dto.Email);

            // BCrypt no acepta un hash vacío: la cuenta blanqueada se informa antes de verificar.
            if (usuario != null && usuario.Activo && usuario.PasswordBlanqueada)
                throw new ConflictException("Tu contraseña fue blanqueada. Creá una nueva con tu número de legajo.");

            if (usuario == null || usuario.PasswordBlanqueada || !_passwordHasher.Verify(dto.Password, usuario.PasswordHash))
                throw new UnauthorizedException("Email o contraseña incorrectos.");

            if (!usuario.Activo)
                throw new UnauthorizedException("El usuario está inactivo.");

            return new AuthResponseDTO { Token = _jwtService.GenerarToken(usuario.Id, usuario.IdRol) };
        }

        /// <inheritdoc />
        public async Task CambiarPasswordAsync(int idUsuario, CambiarPasswordDTO dto)
        {
            var usuario = await _usuarios.GetByIdAsync(idUsuario);
            if (usuario == null || !usuario.Activo)
                throw new UnauthorizedException("La sesión ya no es válida. Iniciá sesión de nuevo.");

            // 400 y no 401: un 401 haría que el cliente cierre la sesión.
            if (!usuario.PasswordBlanqueada
                && (string.IsNullOrEmpty(dto.PasswordActual) || !_passwordHasher.Verify(dto.PasswordActual, usuario.PasswordHash)))
                throw new BadRequestException("La contraseña actual no es correcta.");

            if (!usuario.PasswordBlanqueada && dto.PasswordNueva == dto.PasswordActual)
                throw new BadRequestException("La contraseña nueva tiene que ser distinta de la actual.");

            usuario.PasswordHash = _passwordHasher.Hash(dto.PasswordNueva);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<AuthResponseDTO> CrearPasswordAsync(CrearPasswordDTO dto)
        {
            var usuario = await _usuarios.GetByEmailAsync(dto.Email.Trim());

            // Mensaje único: no revela si el email existe, si el legajo coincide o si la cuenta está blanqueada.
            if (usuario == null
                || !usuario.Activo
                || !usuario.PasswordBlanqueada
                || !string.Equals(usuario.NumeroLegajo, dto.NumeroLegajo.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Los datos no coinciden con una cuenta con la contraseña blanqueada.");

            usuario.PasswordHash = _passwordHasher.Hash(dto.PasswordNueva);
            await _unitOfWork.SaveChangesAsync();

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
            var usuario = await _usuarios.GetByIdAsync(idUsuario);

            if (usuario == null || !usuario.Activo)
                throw new UnauthorizedException("La sesión ya no es válida. Iniciá sesión de nuevo.");

            var rol = await _roles.GetByIdAsync(usuario.IdRol);

            return usuario.ToProfileDTO(rol?.Nombre);
        }
    }
}
