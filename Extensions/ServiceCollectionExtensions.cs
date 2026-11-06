using Plataforma24API.Common.Interfaces;
using Plataforma24API.Data;
using Plataforma24API.Infrastructure.Middleware;
using Plataforma24API.Infrastructure.RateLimiting;
using Plataforma24API.Infrastructure.Security;
using Plataforma24API.Infrastructure.Storage;
using Plataforma24API.Repositories;
using Plataforma24API.Repositories.Interfaces;
using Plataforma24API.Services;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.Extensions
{
    /// <summary>
    /// Raiz de composicion de las dependencias: asocia cada abstraccion con su implementacion.
    /// Es el unico lugar que conoce los tipos concretos de todas las capas.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services
                .AddInfrastructure()
                .AddRepositories()
                .AddServices();
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddSingleton<IFileStorage, LocalFileStorage>();

            services.AddMemoryCache();
            services.AddSingleton<ILimiteIntentos, LimiteIntentosPorCuenta>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IDatosPersonalesRepository, DatosPersonalesRepository>();
            services.AddScoped<IMateriaRepository, MateriaRepository>();
            services.AddScoped<IMaterialRepository, MaterialRepository>();
            services.AddScoped<ITareaRepository, TareaRepository>();
            services.AddScoped<IArchivoRepository, ArchivoRepository>();
            services.AddScoped<IEntregaRepository, EntregaRepository>();
            services.AddScoped<IMaterialArchivoRepository, MaterialArchivoRepository>();
            services.AddScoped<ITareaArchivoRepository, TareaArchivoRepository>();
            services.AddScoped<IEntregaArchivoRepository, EntregaArchivoRepository>();
            services.AddScoped<IPeriodoRepository, PeriodoRepository>();
            services.AddScoped<IBoletinRepository, BoletinRepository>();
            services.AddScoped<IBoletinNotaRepository, BoletinNotaRepository>();
            services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
            services.AddScoped<IAsistenciaDetalleRepository, AsistenciaDetalleRepository>();
            services.AddScoped<IForoTemaRepository, ForoTemaRepository>();
            services.AddScoped<IForoMensajeRepository, ForoMensajeRepository>();
            services.AddScoped<IForoReporteRepository, ForoReporteRepository>();
            services.AddScoped<IPalabraProhibidaRepository, PalabraProhibidaRepository>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<ICursoService, CursoService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IDatosPersonalesService, DatosPersonalesService>();
            services.AddScoped<IMateriaService, MateriaService>();
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<ITareaService, TareaService>();
            services.AddScoped<IArchivoService, ArchivoService>();
            services.AddScoped<IEntregaService, EntregaService>();
            services.AddScoped<IMaterialArchivoService, MaterialArchivoService>();
            services.AddScoped<ITareaArchivoService, TareaArchivoService>();
            services.AddScoped<IEntregaArchivoService, EntregaArchivoService>();
            services.AddScoped<IPeriodoService, PeriodoService>();
            services.AddScoped<IBoletinService, BoletinService>();
            services.AddScoped<IAsistenciaService, AsistenciaService>();
            services.AddScoped<IForoService, ForoService>();
            services.AddScoped<IPalabraProhibidaService, PalabraProhibidaService>();

            return services;
        }
    }
}
