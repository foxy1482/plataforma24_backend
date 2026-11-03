using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Models;

namespace Plataforma24API.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Registra el <see cref="AppDbContext"/> sobre PostgreSQL y asocia cada enum de C# con su tipo enumerado
        /// en la base de datos.
        /// </summary>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    o => o.MapEnum<Sexo>()
                          .MapEnum<Turno>("turno")
                          .MapEnum<Year>("years")
                          .MapEnum<Division>("divisions")
                          .MapEnum<EstadoBoletin>("estado_boletin")
                          .MapEnum<EstadoAsistencia>("estado_asistencia")
                          .MapEnum<ModoCoincidencia>("modo_coincidencia")));

            return services;
        }
    }
}
