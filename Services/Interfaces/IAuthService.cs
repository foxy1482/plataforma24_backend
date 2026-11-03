using Plataforma24API.DTOs.Auth;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Autenticacion y alta de cuentas.</summary>
    public interface IAuthService
    {
        /// <exception cref="Common.Exceptions.UnauthorizedException">Credenciales incorrectas o usuario inactivo.</exception>
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        /// <summary>Crea una cuenta con rol de alumno y devuelve su token.</summary>
        /// <exception cref="Common.Exceptions.BadRequestException">El email o el nombre de usuario ya estan en uso.</exception>
        Task<AuthResponseDTO> RegistroAsync(RegistroDTO dto);

        /// <summary>Devuelve el perfil incluido el nombre del rol.</summary>
        /// <exception cref="Common.Exceptions.UnauthorizedException">El usuario fue eliminado o dado de baja: el token ya no sirve.</exception>
        Task<AuthProfileDTO> ObtenerPerfilAsync(int idUsuario);
    }
}
