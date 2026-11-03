using Plataforma24API.Common.Constants;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;
using System.Security.Claims;

namespace Plataforma24API.Infrastructure.Security
{
    /// <summary>Implementacion de <see cref="ICurrentUser"/> que lee los claims del JWT de la peticion en curso.</summary>
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal Principal =>
            _httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException("No hay un usuario autenticado.");

        public int Id => LeerClaimEntero(AppClaims.IdUsuario);

        public int IdRol => LeerClaimEntero(AppClaims.IdRol);

        public bool EsAdmin => RolIds.EsAdmin(IdRol);

        public bool EsProfesor => RolIds.EsProfesor(IdRol);

        public bool EsPreceptor => RolIds.EsPreceptor(IdRol);

        public bool EsAlumno => RolIds.EsAlumno(IdRol);

        public bool EsPersonal => RolIds.EsPersonal(IdRol);

        public bool TienePermiso(int idUsuarioObjetivo) => EsAdmin || Id == idUsuarioObjetivo;

        private int LeerClaimEntero(string nombre)
        {
            var valor = Principal.FindFirst(nombre)?.Value;

            if (!int.TryParse(valor, out int resultado))
                throw new UnauthorizedException("El token no tiene los datos del usuario. Iniciá sesión de nuevo.");

            return resultado;
        }
    }
}
