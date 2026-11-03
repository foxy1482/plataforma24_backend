using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="PalabraProhibida"/>.</summary>
    public interface IPalabraProhibidaRepository : IRepository<PalabraProhibida>
    {
        Task<List<PalabraProhibida>> ListarOrdenadasAsync();

        /// <summary>Busca en el texto una palabra bloqueada activa (funcion <c>fn_buscar_palabra_prohibida</c>).</summary>
        /// <param name="texto">Texto a revisar.</param>
        /// <returns>La primera palabra encontrada, o <c>null</c> si el texto esta permitido.</returns>
        Task<string?> BuscarPalabraProhibidaAsync(string texto);
    }
}
