using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Plataforma24API.Common.Exceptions;
using Plataforma24API.Common.Interfaces;

namespace Plataforma24API.Infrastructure.RateLimiting
{
    /// <summary>
    /// Ventana fija de un minuto por clave en memoria. Cada entrada expira con su ventana, así que un barrido con
    /// emails inventados no hace crecer la memoria indefinidamente. Con varias instancias de la API, cada una
    /// lleva su propio conteo.
    /// </summary>
    public sealed class LimiteIntentosPorCuenta : ILimiteIntentos
    {
        private static readonly TimeSpan Ventana = TimeSpan.FromMinutes(1);

        private readonly IMemoryCache _cache;
        private readonly int _cupo;
        private readonly Lock _candado = new();

        public LimiteIntentosPorCuenta(IMemoryCache cache, IOptions<RateLimitSettings> settings)
        {
            _cache = cache;
            _cupo = settings.Value.IntentosPorCuentaPorMinuto;
        }

        /// <inheritdoc />
        public void Registrar(string clave)
        {
            var ahora = DateTimeOffset.UtcNow;
            var llave = "intentos:" + clave.Trim().ToLowerInvariant();

            lock (_candado)
            {
                if (!_cache.TryGetValue(llave, out Contador? contador) || contador!.Vence <= ahora)
                {
                    contador = new Contador { Vence = ahora + Ventana };
                    _cache.Set(llave, contador, contador.Vence);
                }

                contador.Intentos++;

                if (contador.Intentos > _cupo)
                    throw new TooManyRequestsException(Math.Max(1, (int)Math.Ceiling((contador.Vence - ahora).TotalSeconds)));
            }
        }

        private sealed class Contador
        {
            public DateTimeOffset Vence { get; init; }
            public int Intentos { get; set; }
        }
    }
}
