using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Archivo"/>.</summary>
    public interface IArchivoRepository : IRepository<Archivo>
    {
        /// <summary>Lista archivos, opcionalmente filtrados por nombre exacto y/o propietario.</summary>
        /// <param name="nombre">Nombre original del archivo; <c>null</c> para no filtrar.</param>
        /// <param name="idUsuario">Propietario; <c>null</c> para no filtrar.</param>
        Task<List<Archivo>> ListarAsync(string? nombre, int? idUsuario);

        /// <summary>Indica si el archivo esta adjunto a alguna tarea o material didactico.</summary>
        Task<bool> EstaAdjuntoATareaOMaterialAsync(int idArchivo);
    }
}
