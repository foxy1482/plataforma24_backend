namespace Plataforma24API.Infrastructure.RateLimiting
{
    /// <summary>
    /// Parametros del limitador de peticiones, enlazados con la seccion <c>RateLimiting</c> de la configuracion.
    /// </summary>
    public sealed class RateLimitSettings
    {
        public const string SectionName = "RateLimiting";

        /// <summary>Limite por usuario, identificado por el claim <c>idUsuario</c> del JWT.</summary>
        public LimiteSettings Autenticado { get; init; } = new() { PeticionesPorMinuto = 50, Rafaga = 75, Cola = 10 };

        /// <summary>
        /// Limite por IP de origen para peticiones sin token valido (login, registro). La red del colegio sale a
        /// internet por una unica IP publica (NAT), asi que el cupo tiene que alcanzar para que decenas de alumnos
        /// inicien sesion a la vez al comienzo de cada turno.
        /// </summary>
        public LimiteSettings Anonimo { get; init; } = new() { PeticionesPorMinuto = 300, Rafaga = 600, Cola = 0 };

        /// <summary>
        /// Intentos por minuto y por email en login y creación de contraseña. Complementa al límite por IP, que
        /// tiene que ser alto porque el colegio comparte una IP pública.
        /// </summary>
        public int IntentosPorCuentaPorMinuto { get; init; } = 8;

        public bool EsValida() => Autenticado.EsValido() && Anonimo.EsValido() && IntentosPorCuentaPorMinuto > 0;
    }

    /// <summary>
    /// Cupo de un token bucket: se recupera una ficha cada <c>60 / PeticionesPorMinuto</c> segundos y el balde
    /// admite hasta <see cref="Rafaga"/> fichas acumuladas.
    /// </summary>
    public sealed class LimiteSettings
    {
        /// <summary>Tasa sostenida: velocidad a la que se recuperan las fichas.</summary>
        public int PeticionesPorMinuto { get; init; }

        /// <summary>
        /// Capacidad del balde. Una pantalla del frontend dispara varias peticiones simultaneas, y un usuario
        /// que estuvo inactivo acumula fichas hasta este tope, por lo que la rafaga absorbe esos picos sin rechazar.
        /// </summary>
        public int Rafaga { get; init; }

        /// <summary>
        /// Peticiones que esperan una ficha en lugar de ser rechazadas cuando el balde se vacia. Con la tasa
        /// por defecto cada posicion de la cola demora ~1,2 s.
        /// </summary>
        public int Cola { get; init; }

        public bool EsValido() => PeticionesPorMinuto > 0 && Rafaga >= PeticionesPorMinuto && Cola >= 0;
    }
}
