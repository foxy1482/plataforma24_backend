using Plataforma24API.DTOs.Entregas;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Archivos adjuntos a las entregas.</summary>
    public interface IEntregaArchivoService
    {
        /// <summary>Lista los adjuntos de una entrega. La ven su autor, los profesores y los administradores.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">La entrega no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El usuario no puede ver esa entrega.</exception>
        Task<IEnumerable<EntregaArchivoDTO>> ListarAsync(int idEntrega);

        /// <summary>Adjunta un archivo propio a una entrega propia.</summary>
        /// <exception cref="Common.Exceptions.BadRequestException">La entrega o el archivo no existen, el archivo no es del usuario o ya estaba adjunto.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">La entrega es de otro usuario.</exception>
        Task VincularAsync(VincEntregaArchivoDTO dto);

        /// <exception cref="Common.Exceptions.ForbiddenException">La entrega o el archivo son de otro usuario.</exception>
        /// <exception cref="Common.Exceptions.NotFoundException">El archivo no esta adjunto a la entrega.</exception>
        Task DesvincularAsync(int idEntrega, int idArchivo);

        /// <summary>Quita todos los adjuntos de una entrega.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">La entrega no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">La entrega es de otro usuario.</exception>
        Task DesvincularTodosAsync(int idEntrega);
    }
}
