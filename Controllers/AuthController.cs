using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.Common.Interfaces;
using Plataforma24API.DTOs.Auth;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Controllers
{
    /// <summary>Inicio de sesion, registro y perfil del usuario autenticado.</summary>
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUser _currentUser;

        public AuthController(IAuthService authService, ICurrentUser currentUser)
        {
            _authService = authService;
            _currentUser = currentUser;
        }

        /// <summary>Devuelve los datos de la cuenta del usuario autenticado.</summary>
        /// <response code="200">Perfil del usuario.</response>
        /// <response code="401">Token ausente o invalido.</response>
        // GET /auth/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<AuthProfileDTO>> Profile()
        {
            return Ok(await _authService.ObtenerPerfilAsync(_currentUser.Id));
        }

        /// <summary>Inicia sesion y devuelve un token JWT.</summary>
        /// <param name="dto">Email y contrasena.</param>
        /// <response code="200">Token de acceso.</response>
        /// <response code="401">Credenciales incorrectas o usuario inactivo.</response>
        // POST /auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
        {
            return Ok(await _authService.LoginAsync(dto));
        }

        /// <summary>Crea una cuenta de alumno y devuelve su token.</summary>
        /// <param name="dto">Email, nombre de usuario y contrasena.</param>
        /// <response code="200">Token de acceso.</response>
        /// <response code="400">El email o el nombre de usuario ya estan en uso.</response>
        // POST /auth/registro
        [HttpPost("registro")]
        public async Task<ActionResult<AuthResponseDTO>> Registro([FromBody] RegistroDTO dto)
        {
            return Ok(await _authService.RegistroAsync(dto));
        }
    }
}
