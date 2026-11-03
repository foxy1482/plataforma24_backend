using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="EntregaArchivo"/>.</summary>
    public interface IEntregaArchivoRepository : IRepository<EntregaArchivo>
    {
        /// <summary>Lista los archivos adjuntos a una entrega.</summary>
        Task<List<Archivo>> ListarArchivosAsync(int idEntrega);

        Task<EntregaArchivo?> GetVinculoAsync(int idEntrega, int idArchivo);

        Task<bool> ExisteVinculoAsync(int idEntrega, int idArchivo);

        /// <summary>Devuelve, rastreados, todos los vinculos de una entrega para poder eliminarlos.</summary>
        Task<List<EntregaArchivo>> ListarVinculosAsync(int idEntrega);

        void RemoverVarios(IEnumerable<EntregaArchivo> vinculos);
    }
}
