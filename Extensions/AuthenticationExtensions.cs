using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Plataforma24API.Infrastructure.Security;
using System.Text;

namespace Plataforma24API.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Registra la autenticacion con JWT. Los parametros se leen una sola vez de la seccion <c>Jwt</c> y se
        /// validan al iniciar la aplicacion, de modo que una configuracion incompleta falle al arrancar y no en
        /// el primer login.
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<JwtSettings>()
                .Bind(config.GetSection(JwtSettings.SectionName))
                .Validate(settings => settings.EsValida(),
                    "La configuración Jwt es inválida: se requieren Key (mínimo 32 bytes), Issuer y Audience.")
                .ValidateOnStart();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtSettings>>((options, jwt) =>
                {
                    var settings = jwt.Value;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = settings.Issuer,
                        ValidAudience = settings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
                    };
                });

            return services;
        }
    }
}
