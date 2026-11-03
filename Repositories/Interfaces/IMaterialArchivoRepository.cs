using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="MaterialArchivo"/>.</summary>
    public interface IMaterialArchivoRepository : IRepository<MaterialArchivo>
    {
        /// <summary>Lista los archivos asociados a un material.</summary>
        Task<List<Archivo>> ListarArchivosAsync(int idMaterial);

        Task<MaterialArchivo?> GetVinculoAsync(int idMaterial, int idArchivo);

        Task<bool> ExisteVinculoAsync(int idMaterial, int idArchivo);
    }
}
