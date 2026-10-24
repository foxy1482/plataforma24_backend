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
            var rolTokenClaim = user.FindFirst("idRol")?.Value;
            if (rolTokenClaim == null) return false;

            if (int.TryParse(rolTokenClaim, out int rolToken))
            {
                return rolToken >= 3 && rolToken <= 6;
            }
            return false;
        }
        public static int? GetRol(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("idRol")?.Value;
            return int.TryParse(claim, out int rol) ? rol : null;
        }

        public static bool EsProfe(this ClaimsPrincipal user)
        {
            var rol = user.GetRol();
            return rol == 2;
        }

        // Rol 7 = preceptor (nuevo en la v2)
        public static bool EsPreceptor(this ClaimsPrincipal user)
        {
            return user.GetRol() == 7;
        }

        // Rol 1 = alumno, rol 4 = adminAlumno (los dos tienen prefijo ALU en la base)
        public static bool EsAlumno(this ClaimsPrincipal user)
        {
            var rol = user.GetRol();
            return rol == 1 || rol == 4;
        }

        // Personal de la escuela: administración, profesores y preceptores.
        // Es quien ve boletines/asistencia de otros y modera el foro.
        public static bool EsPersonal(this ClaimsPrincipal user)
        {
            return user.EsAdmin() || user.EsProfe() || user.EsPreceptor();
        }

        public static bool TienePermiso(this ClaimsPrincipal user, int idUsuarioObjetivo)
        {
            var idTokenClaim = user.FindFirst("idUsuario")?.Value;

            if (idTokenClaim == null) return false;

            // Si es admin, tiene permiso para todo
            if (user.EsAdmin()) return true;

            if (int.TryParse(idTokenClaim, out int idToken))
            {
                return idToken == idUsuarioObjetivo;
            }

            return false;
        }

        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("idUsuario")?.Value;
            return int.TryParse(claim, out int id) ? id : null;
        }
    }
}