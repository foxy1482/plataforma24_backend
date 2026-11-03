namespace Plataforma24API.Common.Exceptions
{
    /// <summary>
    /// Base de las excepciones de negocio. <c>GlobalExceptionHandler</c> las traduce a la respuesta HTTP que corresponde,
    /// de modo que los servicios no dependan de ASP.NET para informar un error.
    /// </summary>
    public abstract class AppException : Exception
    {
        protected AppException(string mensaje) : base(mensaje) { }
    }

    /// <summary>El recurso pedido no existe. Se traduce a HTTP 404.</summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string mensaje) : base(mensaje) { }
    }

    /// <summary>Los datos recibidos no cumplen una regla de negocio. Se traduce a HTTP 400.</summary>
    public class BadRequestException : AppException
    {
        public BadRequestException(string mensaje) : base(mensaje) { }
    }

    /// <summary>El usuario esta autenticado pero no puede realizar la accion. Se traduce a HTTP 403.</summary>
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string mensaje = "No tenés permisos para realizar esta acción.") : base(mensaje) { }
    }

    /// <summary>La operacion choca con el estado actual del recurso. Se traduce a HTTP 409.</summary>
    public class ConflictException : AppException
    {
        public ConflictException(string mensaje) : base(mensaje) { }
    }

    /// <summary>Faltan credenciales validas. Se traduce a HTTP 401.</summary>
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string mensaje = "Tenés que iniciar sesión.") : base(mensaje) { }
    }
}
