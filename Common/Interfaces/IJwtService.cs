namespace Plataforma24API.Common.Interfaces
{
    /// <summary>Emision de tokens JWT de acceso.</summary>
    public interface IJwtService
    {
        /// <summary>Genera un token firmado que identifica al usuario y su rol.</summary>
        /// <param name="idUsuario">Identificador del usuario.</param>
        /// <param name="idRol">Identificador de su rol.</param>
        string GenerarToken(int idUsuario, int idRol);
    }
}
