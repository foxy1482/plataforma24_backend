using Plataforma24API.DTOs.Foro;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Palabras bloqueadas en el foro y verificacion de textos.</summary>
    public interface IPalabraProhibidaService
    {
        Task<IEnumerable<PalabraDTO>> ListarAsync();

        /// <summary>Bloquea una palabra. Queda registrado el usuario que la agrego.</summary>
        Task<PalabraDTO> CrearAsync(CrearPalabraDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La palabra no existe.</exception>
        Task ActualizarAsync(int id, ModificarPalabraDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La palabra no existe.</exception>
        Task EliminarAsync(int id);

        /// <summary>Indica si un texto contiene alguna palabra bloqueada activa, sin publicarlo.</summary>
        Task<VerificacionTextoDTO> VerificarAsync(VerificarTextoDTO dto);
    }
}
