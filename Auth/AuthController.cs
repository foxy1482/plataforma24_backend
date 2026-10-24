using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma24API.DTOs.Auth;
using Plataforma24API.Models;
using System.Security.Claims;
namespace Plataforma24API.Auth
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var claim = User.FindFirst("idUsuario");

            if (claim == null)
            {
                return Unauthorized("No se encontró el id del usuario en el token proporcionado.");
            }

            if (!int.TryParse(claim.Value, out int idUsuario))
            {
                return BadRequest("El id del usuario en el token no es válido");
            }

            var perfil = _authService.Profile(idUsuario);
            return Ok(perfil);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            try
            {
                var token = _authService.Login(dto).Token;

                return Ok(new AuthResponseDTO { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("registro")]
        public IActionResult Registro(RegisterDTO dto)
        {
            try
            {
                var token = _authService.Registro(dto).Token;

                return Ok(new AuthResponseDTO { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}