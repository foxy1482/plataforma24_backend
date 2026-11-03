namespace Plataforma24API.Common.Constants
{
    /// <summary>
    /// Nombres de los claims propios que se incluyen en el JWT.
    /// Centralizarlos evita que el emisor del token y quienes lo leen se desincronicen.
    /// </summary>
    public static class AppClaims
    {
        /// <summary>Identificador del usuario autenticado.</summary>
        public const string IdUsuario = "idUsuario";

        /// <summary>Identificador del rol del usuario autenticado.</summary>
        public const string IdRol = "idRol";
    }
}
