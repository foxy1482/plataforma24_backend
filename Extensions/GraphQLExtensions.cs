using Plataforma24API.GraphQL;

namespace Plataforma24API.Extensions
{
    public static class GraphQLExtensions
    {
        /// <summary>
        /// Registra el servidor GraphQL con las consultas y mutaciones minimas. Exige el mismo JWT que la API REST.
        /// </summary>
        public static IServiceCollection AddGraphQLApi(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddType<CursoType>()
                .AddErrorFilter<AppExceptionErrorFilter>();

            return services;
        }
    }
}
