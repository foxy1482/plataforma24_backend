using Plataforma24API.DTOs.Foro;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>
    /// Foro de la institucion. El personal modera y ve todos los temas; el resto solo ve los temas
    /// generales y los de su propio curso.
    /// </summary>
    public interface IForoService
    {
        /// <param name="idCurso">Limita a los temas de ese curso; <c>null</c> para no filtrar.</param>
        Task<IEnumerable<ForoTemaDTO>> ListarTemasAsync(int? idCurso);

        /// <summary>Abre un tema y, si se indica, publica su primer mensaje en la misma transaccion.</summary>
        /// <returns>Id del tema.</returns>
        Task<int> CrearTemaAsync(CrearTemaDTO dto);

        /// <summary>Fija o cierra un tema.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El tema no existe o fue eliminado.</exception>
        Task ModerarTemaAsync(int idTema, ModificarTemaDTO dto);

        /// <summary>Elimina un tema de forma logica. Puede hacerlo su autor o el personal.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El tema no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El usuario no es el autor ni personal.</exception>
        Task EliminarTemaAsync(int idTema);

        /// <exception cref="Common.Exceptions.NotFoundException">El tema no existe o fue eliminado.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El tema es de otro curso.</exception>
        Task<IEnumerable<ForoMensajeDTO>> ListarMensajesAsync(int idTema);

        /// <exception cref="Common.Exceptions.NotFoundException">El tema no existe o fue eliminado.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El tema es de otro curso.</exception>
        /// <exception cref="Common.Exceptions.ConflictException">El tema esta cerrado.</exception>
        /// <returns>Id del mensaje.</returns>
        Task<int> CrearMensajeAsync(int idTema, CrearMensajeDTO dto);

        /// <summary>Edita un mensaje. Solo puede hacerlo su autor.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El mensaje no existe o fue eliminado.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El usuario no es el autor.</exception>
        Task EditarMensajeAsync(int idMensaje, ModificarMensajeDTO dto);

        /// <summary>Elimina un mensaje de forma logica. Puede hacerlo su autor o el personal.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El mensaje no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">El usuario no es el autor ni personal.</exception>
        Task EliminarMensajeAsync(int idMensaje);

        /// <exception cref="Common.Exceptions.NotFoundException">El mensaje no existe o no es visible para el usuario.</exception>
        /// <exception cref="Common.Exceptions.ConflictException">El usuario ya reporto ese mensaje.</exception>
        /// <returns>Id del reporte.</returns>
        Task<int> ReportarMensajeAsync(int idMensaje, CrearReporteDTO dto);

        /// <param name="soloPendientes">Si es <c>true</c>, omite los reportes ya resueltos.</param>
        Task<IEnumerable<ReporteDTO>> ListarReportesAsync(bool soloPendientes);

        /// <summary>Marca un reporte como resuelto por el usuario autenticado.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El reporte no existe.</exception>
        Task ResolverReporteAsync(int idReporte);
    }
}
