using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="ForoReporte"/>.</summary>
    public interface IForoReporteRepository : IRepository<ForoReporte>
    {
        /// <summary>Lista los reportes con su mensaje, del mas reciente al mas antiguo.</summary>
        /// <param name="soloPendientes">Si es <c>true</c>, omite los reportes ya resueltos.</param>
        Task<List<(ForoReporte Reporte, ForoMensaje Mensaje)>> ListarConMensajeAsync(bool soloPendientes);

        /// <summary>Indica si el usuario ya reporto ese mensaje.</summary>
        Task<bool> ExisteAsync(int idMensaje, int idReportante);
    }
}
