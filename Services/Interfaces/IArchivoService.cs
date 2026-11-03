using Plataforma24API.DTOs.Archivos;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Subida, descarga y administracion de archivos.</summary>
    public interface IArchivoService
    {
        /// <param name="nombre">Nombre original exacto; <c>null</c> para no filtrar.</param>
        /// <param name="idUsuario">Propietario; <c>null</c> para no filtrar.</param>
        Task<IEnumerable<ArchivoDTO>> ListarAsync(string? nombre, int? idUsuario);

        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no existe.</exception>
        Task<ArchivoDTO> BuscarAsync(int id);

        /// <summary>Guarda el archivo y registra sus metadatos. El propietario es el usuario autenticado.</summary>
        /// <param name="archivo">Contenido y datos del archivo recibido.</param>
        /// <exception cref="Common.Exceptions.BadRequestException">El archivo esta vacio o la subcarpeta es invalida.</exception>
        Task<ArchivoDTO> SubirAsync(ArchivoParaSubir archivo);

        /// <summary>
        /// Abre el archivo para descargarlo. Pueden verlo su propietario, los administradores, los profesores
        /// (si el propietario es alumno) y cualquier usuario si el archivo esta adjunto a una tarea o material.
        /// </summary>
        /// <exception cref="Common.Exceptions.NotFoundException">No existe el registro o el archivo fisico.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El usuario no tiene permiso para verlo.</exception>
        Task<ArchivoContenido> ObtenerContenidoAsync(int id);

        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no existe.</exception>
        Task ActualizarMetadatosAsync(int id, ModificarArchivoDTO dto);

        /// <summary>Elimina el registro y el archivo fisico.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no existe.</exception>
        Task EliminarAsync(int id);
    }
}
