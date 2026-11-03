using Plataforma24API.DTOs.Asistencias;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Planillas de asistencia por curso y resumen de inasistencias por alumno.</summary>
    public interface IAsistenciaService
    {
        /// <summary>Abre la planilla de un curso para una fecha, con una fila por alumno.</summary>
        /// <returns>Id de la planilla.</returns>
        Task<int> AbrirPlanillaAsync(AbrirAsistenciaDTO dto);

        /// <param name="idCurso">Curso.</param>
        /// <param name="fecha">Dia consultado; si es <c>null</c>, el dia de hoy.</param>
        /// <exception cref="Common.Exceptions.NotFoundException">No hay planilla abierta para ese curso y fecha.</exception>
        Task<PlanillaAsistenciaDTO> ObtenerPlanillaAsync(int idCurso, DateOnly? fecha);

        /// <param name="idCurso">Curso.</param>
        /// <param name="desde">Fecha inicial inclusive; <c>null</c> para no acotar.</param>
        /// <param name="hasta">Fecha final inclusive; <c>null</c> para no acotar.</param>
        Task<IEnumerable<AsistenciaDTO>> ListarPlanillasAsync(int idCurso, DateOnly? desde, DateOnly? hasta);

        /// <summary>Modifica el estado u observacion de uno o mas alumnos de una planilla abierta.</summary>
        /// <param name="idAsistencia">Planilla.</param>
        /// <param name="cambios">Cambios por alumno; no puede estar vacio.</param>
        /// <exception cref="Common.Exceptions.NotFoundException">La planilla no existe.</exception>
        /// <exception cref="Common.Exceptions.ConflictException">La planilla esta cerrada.</exception>
        /// <exception cref="Common.Exceptions.BadRequestException">No hay cambios o un alumno no figura en la planilla.</exception>
        Task MarcarAsistenciaAsync(int idAsistencia, List<ModificarAsistenciaDetalleDTO> cambios);

        /// <summary>Cierra, reabre o anota una planilla. Solo un administrador puede reabrir una planilla cerrada.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">La planilla no existe.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">Se intento reabrir sin ser administrador.</exception>
        Task ActualizarPlanillaAsync(int idAsistencia, ModificarAsistenciaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La planilla no existe.</exception>
        Task EliminarPlanillaAsync(int idAsistencia);

        /// <summary>Inasistencias de un alumno, anuales y por bimestre. El personal ve cualquiera; un alumno, solo el suyo.</summary>
        /// <param name="idAlumno">Alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        /// <exception cref="Common.Exceptions.ForbiddenException">Un alumno consulto a otro.</exception>
        Task<ResumenAsistenciaDTO> ResumenAlumnoAsync(int idAlumno, int? ciclo);
    }
}
