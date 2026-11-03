using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Plataforma24API.Infrastructure.Security
{
    /// <summary>Emite tokens JWT firmados con HMAC-SHA256 segun la configuracion de <see cref="JwtSettings"/>.</summary>
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        /// <inheritdoc />
        public string GenerarToken(int idUsuario, int idRol)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(AppClaims.IdUsuario, idUsuario.ToString()),
                new Claim(AppClaims.IdRol, idRol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_settings.ExpiracionDias),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
