using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Curso"/>.</summary>
    public interface ICursoRepository : IRepository<Curso>
    {
        /// <param name="ano">Ano de cursada; <c>null</c> para no filtrar.</param>
        /// <param name="turno">Turno; <c>null</c> para no filtrar.</param>
        Task<List<Curso>> ListarAsync(Year? ano, Turno? turno);

        /// <summary>Indica si ya existe un curso con la misma combinacion de ano, division, turno y ciclo.</summary>
        Task<bool> ExisteAsync(Year ano, Division division, Turno turno, int cicloLectivo);
    }
}
