using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Plataforma24API.Common.Exceptions;

namespace Plataforma24API.Infrastructure.Middleware
{
    /// <summary>
    /// Convierte cualquier excepcion no capturada en una respuesta <see cref="ProblemDetails"/>.
    /// Las excepciones de negocio, y las que provienen de reglas implementadas en la base de datos
    /// se traducen a su codigo HTTP sin detalles
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        /// <summary>SQLSTATE propio que lanza el trigger del foro cuando el texto contiene una palabra prohibida.</summary>
        private const string SqlStateTextoProhibido = "FORO1";

        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var problema = Traducir(exception);

            if (problema.Status == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Error no controlado en {Metodo} {Ruta}",
                    httpContext.Request.Method, httpContext.Request.Path);
            }
            else
            {
                _logger.LogInformation("{Tipo}: {Mensaje}", exception.GetType().Name, exception.Message);
            }

            httpContext.Response.StatusCode = problema.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);

            return true;
        }

        private static ProblemDetails Traducir(Exception exception)
        {
            if (exception is AppException app)
            {
                return app switch
                {
                    NotFoundException => Crear(StatusCodes.Status404NotFound, "No encontrado", app.Message),
                    BadRequestException => Crear(StatusCodes.Status400BadRequest, "Pedido inválido", app.Message),
                    ForbiddenException => Crear(StatusCodes.Status403Forbidden, "Acción no permitida", app.Message),
                    ConflictException => Crear(StatusCodes.Status409Conflict, "Conflicto", app.Message),
                    UnauthorizedException => Crear(StatusCodes.Status401Unauthorized, "No autenticado", app.Message),
                    _ => Crear(StatusCodes.Status400BadRequest, "Pedido inválido", app.Message)
                };
            }

            var pg = BuscarPostgres(exception);
            if (pg == null) return ErrorInterno();

            return pg.SqlState switch
            {
                PostgresErrorCodes.CheckViolation =>
                    Crear(StatusCodes.Status400BadRequest, "Regla de negocio no cumplida", DetalleReglaNegocio(pg)),

                PostgresErrorCodes.InsufficientPrivilege =>
                    Crear(StatusCodes.Status403Forbidden, "Acción no permitida", pg.MessageText),

                SqlStateTextoProhibido =>
                    ConSugerencia(Crear(StatusCodes.Status422UnprocessableEntity, "Texto no permitido", pg.MessageText), pg.Hint),

                PostgresErrorCodes.UniqueViolation =>
                    Crear(StatusCodes.Status409Conflict, "Registro duplicado", "Ya existe un registro con esos datos."),

                PostgresErrorCodes.ForeignKeyViolation =>
                    Crear(StatusCodes.Status400BadRequest, "Referencia inválida",
                        "Hace referencia a un registro que no existe, o todavía tiene datos asociados."),

                _ => ErrorInterno()
            };
        }

        /// <summary>Recorre las excepciones internas hasta encontrar la que genero el motor de base de datos.</summary>
        private static PostgresException? BuscarPostgres(Exception exception)
        {
            for (Exception? e = exception; e != null; e = e.InnerException)
            {
                if (e is PostgresException pg) return pg;
            }

            return null;
        }

        /// <summary>
        /// Los triggers de la base lanzan mensajes pensados para el usuario y se muestran tal cual; en cambio, el texto
        /// de una restriccion CHECK incluye nombres de tablas y restricciones que no deben exponerse.
        /// </summary>
        private static string DetalleReglaNegocio(PostgresException pg)
        {
            return pg.ConstraintName == null
                ? pg.MessageText
                : "Los datos no cumplen una regla de negocio. Revisá los valores enviados.";
        }

        private static ProblemDetails Crear(int status, string titulo, string detalle)
        {
            return new ProblemDetails { Status = status, Title = titulo, Detail = detalle };
        }

        private static ProblemDetails ConSugerencia(ProblemDetails problema, string? sugerencia)
        {
            if (!string.IsNullOrEmpty(sugerencia)) problema.Extensions["sugerencia"] = sugerencia;
            return problema;
        }

        private static ProblemDetails ErrorInterno()
        {
            return Crear(StatusCodes.Status500InternalServerError, "Error interno",
                "Ocurrió un error inesperado. Si se repite, avisá al administrador.");
        }
    }
}
