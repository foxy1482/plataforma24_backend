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

        /// <summary>Cambia la contrasena verificando la actual (salvo que este blanqueada).</summary>
        /// <exception cref="Common.Exceptions.BadRequestException">La contrasena actual no coincide o la nueva es igual.</exception>
        Task CambiarPasswordAsync(int idUsuario, CambiarPasswordDTO dto);

        /// <summary>Crea la contrasena de una cuenta blanqueada, verificando email y legajo, y devuelve su token.</summary>
        /// <exception cref="Common.Exceptions.BadRequestException">Los datos no coinciden con una cuenta blanqueada.</exception>
        Task<AuthResponseDTO> CrearPasswordAsync(CrearPasswordDTO dto);

        /// <summary>Devuelve el perfil incluido el nombre del rol.</summary>
        /// <exception cref="Common.Exceptions.UnauthorizedException">El usuario fue eliminado o dado de baja: el token ya no sirve.</exception>
        Task<AuthProfileDTO> ObtenerPerfilAsync(int idUsuario);
    }
}
