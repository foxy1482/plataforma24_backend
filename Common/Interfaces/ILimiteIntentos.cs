namespace Plataforma24API.Common.Interfaces
{
    /// <summary>
    /// Límite de intentos por cuenta (no por IP: el colegio comparte una IP pública). Frena la prueba masiva de
    /// contraseñas o legajos contra un mismo email sin bloquear al resto de los usuarios de la red.
    /// </summary>
    public interface ILimiteIntentos
    {
        /// <summary>Cuenta un intento para la clave y lanza si se superó el cupo de la ventana actual.</summary>
        /// <exception cref="Common.Exceptions.TooManyRequestsException">Se superó el cupo por minuto.</exception>
        void Registrar(string clave);
    }
}
