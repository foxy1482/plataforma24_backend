using Microsoft.AspNetCore.Authorization;
using Plataforma24API.Common.Constants;
using Plataforma24API.Infrastructure.Security;
using System.Security.Claims;

namespace Plataforma24API.Extensions
{
    public static class AuthorizationExtensions
    {
        /// <summary>Registra las politicas de <see cref="Policies"/>, que se resuelven con el claim de rol del JWT.</summary>
        public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.Admin, policy => RequerirRol(policy, RolIds.EsAdmin))
                .AddPolicy(Policies.AdminOPreceptor, policy => RequerirRol(policy, RolIds.EsAdminOPreceptor))
                .AddPolicy(Policies.AdminOProfesor, policy => RequerirRol(policy, RolIds.EsAdminOProfesor))
                .AddPolicy(Policies.Personal, policy => RequerirRol(policy, RolIds.EsPersonal));

            return services;
        }

        private static void RequerirRol(AuthorizationPolicyBuilder policy, Func<int, bool> rolPermitido)
        {
            policy
                .RequireAuthenticatedUser()
                .RequireAssertion(contexto => rolPermitido(LeerRol(contexto.User)));
        }

        /// <summary>Devuelve el rol del token o 0 (ningun rol valido) si el claim falta o esta mal formado.</summary>
        private static int LeerRol(ClaimsPrincipal usuario)
        {
            return int.TryParse(usuario.FindFirst(AppClaims.IdRol)?.Value, out int rol) ? rol : 0;
        }
    }
}
