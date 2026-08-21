using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Plataforma24API.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddJwtAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            var jwtKey = config["Jwt:Key"];

            services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],

                    IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            return services;
        }

        public static bool EsAdmin(this ClaimsPrincipal user)
        {
            var rolTokenClaim = user.FindFirst("numRol")?.Value;
            if (rolTokenClaim == null) return false;

            if (int.TryParse(rolTokenClaim, out int rolToken))
            {
                return rolToken >= 3 && rolToken <= 6;
            }
            return false;
        }
        public static int? GetRol(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("numRol")?.Value;
            return int.TryParse(claim, out int rol) ? rol : null;
        }

        public static bool EsProfe(this ClaimsPrincipal user)
        {
            var rol = user.GetRol();
            return rol == 2;
        }

        public static bool TienePermiso(this ClaimsPrincipal user, int legajoObjetivo)
        {
            var legajoTokenClaim = user.FindFirst("legajoUs")?.Value;

            if (legajoTokenClaim == null) return false;

            // Si es admin, tiene permiso para todo
            if (user.EsAdmin()) return true;

            if (int.TryParse(legajoTokenClaim, out int legajoToken))
            {
                return legajoToken == legajoObjetivo;
            }

            return false;
        }

        public static int? GetLegajo(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("legajoUs")?.Value;
            return int.TryParse(claim, out int legajo) ? legajo : null;
        }
    }
}
