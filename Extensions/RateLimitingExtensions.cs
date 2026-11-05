using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Plataforma24API.Common.Constants;
using Plataforma24API.Infrastructure.RateLimiting;
using System.Globalization;
using System.Threading.RateLimiting;

namespace Plataforma24API.Extensions
{
    public static class RateLimitingExtensions
    {
        private const string CategoriaLog = "Plataforma24API.RateLimiting";

        /// <summary>
        /// Registra un limitador global con token bucket particionado: una particion por usuario autenticado y una
        /// por IP para el trafico anonimo. Los parametros se validan al iniciar la aplicacion.
        /// </summary>
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<RateLimitSettings>()
                .Bind(config.GetSection(RateLimitSettings.SectionName))
                .Validate(settings => settings.EsValida(),
                    "La configuración RateLimiting es inválida: PeticionesPorMinuto debe ser positivo, " +
                    "Rafaga no puede ser menor que PeticionesPorMinuto y Cola no puede ser negativa.")
                .ValidateOnStart();

            services.AddRateLimiter(_ => { });

            // Se configura aparte para poder leer RateLimitSettings ya enlazado y validado desde el contenedor.
            services.AddOptions<RateLimiterOptions>()
                .Configure<IOptions<RateLimitSettings>>((options, settings) =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                    options.GlobalLimiter = CrearLimitador(settings.Value);
                    options.OnRejected = RechazarAsync;
                });

            return services;
        }

        private static PartitionedRateLimiter<HttpContext> CrearLimitador(RateLimitSettings settings)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var idUsuario = LeerIdUsuario(context);

                if (idUsuario != null)
                {
                    return RateLimitPartition.GetTokenBucketLimiter(
                        $"usuario:{idUsuario}", _ => CrearBalde(settings.Autenticado));
                }

                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "desconocida";
                return RateLimitPartition.GetTokenBucketLimiter($"ip:{ip}", _ => CrearBalde(settings.Anonimo));
            });
        }

        /// <summary>
        /// Una ficha por periodo en lugar de lotes: la recuperacion es continua (1 ficha cada 1,2 s a 50/min)
        /// y no hay un reinicio brusco en el borde de la ventana como en un contador fijo.
        /// </summary>
        private static TokenBucketRateLimiterOptions CrearBalde(LimiteSettings limite)
        {
            return new TokenBucketRateLimiterOptions
            {
                TokenLimit = limite.Rafaga,
                TokensPerPeriod = 1,
                ReplenishmentPeriod = TimeSpan.FromMilliseconds(60_000d / limite.PeticionesPorMinuto),
                QueueLimit = limite.Cola,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true
            };
        }

        /// <summary>
        /// Solo se confia en el claim cuando el JWT paso la validacion de firma; <c>UseRateLimiter</c> debe ir
        /// despues de <c>UseAuthentication</c> para que <c>HttpContext.User</c> ya este poblado.
        /// </summary>
        private static string? LeerIdUsuario(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true) return null;

            var valor = context.User.FindFirst(AppClaims.IdUsuario)?.Value;
            return string.IsNullOrEmpty(valor) ? null : valor;
        }

        private static async ValueTask RechazarAsync(OnRejectedContext rechazo, CancellationToken cancellationToken)
        {
            var context = rechazo.HttpContext;

            var segundos = rechazo.Lease.TryGetMetadata(MetadataName.RetryAfter, out var espera)
                ? Math.Max(1, (int)Math.Ceiling(espera.TotalSeconds))
                : 1;
            context.Response.Headers.RetryAfter = segundos.ToString(CultureInfo.InvariantCulture);

            context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(CategoriaLog)
                .LogWarning("Límite de peticiones excedido en {Metodo} {Ruta} por {Origen}",
                    context.Request.Method,
                    context.Request.Path,
                    LeerIdUsuario(context) is { } id ? $"usuario {id}" : $"IP {context.Connection.RemoteIpAddress}");

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Demasiadas peticiones",
                Detail = $"Superaste el límite de peticiones. Reintentá en {segundos} segundo(s)."
            }, cancellationToken);
        }
    }
}
