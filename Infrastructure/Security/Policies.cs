namespace Plataforma24API.Infrastructure.Security
{
    /// <summary>
    /// Nombres de las politicas de autorizacion que se usan en <c>[Authorize(Policy = ...)]</c>.
    /// Su definicion esta en <c>AuthorizationExtensions</c>.
    /// </summary>
    public static class Policies
    {
        public const string Admin = "Admin";
        public const string AdminOPreceptor = "AdminOPreceptor";
        public const string AdminOProfesor = "AdminOProfesor";
        public const string Personal = "Personal";
    }
}
