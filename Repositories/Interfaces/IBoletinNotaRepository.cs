using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="BoletinNota"/>.</summary>
    public interface IBoletinNotaRepository : IRepository<BoletinNota>
    {
        /// <summary>Busca la nota de una materia dentro de un boletin.</summary>
        /// <returns>La nota rastreada, o <c>null</c> si el boletin no incluye esa materia.</returns>
        Task<BoletinNota?> GetAsync(int idBoletin, int idMateria);
    }
}
