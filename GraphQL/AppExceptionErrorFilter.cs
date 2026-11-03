using HotChocolate;
using HotChocolate.Execution;
using Plataforma24API.Common.Exceptions;

namespace Plataforma24API.GraphQL
{
    /// <summary>
    /// Muestra al cliente el mensaje de las excepciones de negocio y su codigo (por ejemplo <c>NOT_FOUND</c>);
    /// cualquier otro error conserva el mensaje generico de HotChocolate para no filtrar detalles internos.
    /// </summary>
    public class AppExceptionErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is not AppException excepcion) return error;

            return ErrorBuilder.FromError(error)
                .SetMessage(excepcion.Message)
                .SetCode(Codigo(excepcion))
                .Build();
        }

        private static string Codigo(AppException excepcion)
        {
            return excepcion switch
            {
                NotFoundException => "NOT_FOUND",
                ForbiddenException => "FORBIDDEN",
                UnauthorizedException => "UNAUTHENTICATED",
                ConflictException => "CONFLICT",
                _ => "BAD_REQUEST"
            };
        }
    }
}
