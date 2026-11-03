using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Plataforma24API.Extensions
{
    public static class SwaggerExtensions
    {
        /// <summary>
        /// Registra Swagger con el esquema de seguridad Bearer y los comentarios XML del proyecto,
        /// de modo que la documentacion del codigo se muestre en la interfaz.
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Plataforma 24 API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Pegá el token JWT que devuelve /auth/login (sin la palabra Bearer)."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });

                var archivoXml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                if (File.Exists(archivoXml)) options.IncludeXmlComments(archivoXml);
            });

            return services;
        }
    }
}
