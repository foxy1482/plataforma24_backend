using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="PeriodoBimestral"/>.</summary>
    public interface IPeriodoRepository : IRepository<PeriodoBimestral>
    {
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        Task<List<PeriodoBimestral>> ListarAsync(int? ciclo);

        Task<bool> ExisteAsync(int cicloLectivo, short bimestre);
    }
}
