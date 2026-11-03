using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="TareaArchivo"/>.</summary>
    public interface ITareaArchivoRepository : IRepository<TareaArchivo>
    {
        /// <summary>Lista los archivos asociados a una tarea.</summary>
        Task<List<Archivo>> ListarArchivosAsync(int idTarea);

        Task<TareaArchivo?> GetVinculoAsync(int idTarea, int idArchivo);

        Task<bool> ExisteVinculoAsync(int idTarea, int idArchivo);
    }
}
