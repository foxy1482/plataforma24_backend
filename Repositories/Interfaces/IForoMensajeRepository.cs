using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="ForoMensaje"/>.</summary>
    public interface IForoMensajeRepository : IRepository<ForoMensaje>
    {
        /// <summary>Lista los mensajes de un tema en orden cronologico.</summary>
        Task<List<VForoMensaje>> ListarVistaPorTemaAsync(int idTema);
    }
}
