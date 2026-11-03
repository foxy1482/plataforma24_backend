using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Asistencia"/>.</summary>
    public interface IAsistenciaRepository : IRepository<Asistencia>
    {
        /// <summary>
        /// Abre la planilla del dia con una fila por cada alumno del curso (funcion <c>fn_abrir_asistencia</c>).
        /// </summary>
        /// <param name="idCurso">Curso.</param>
        /// <param name="fecha">Dia de la planilla.</param>
        /// <param name="idUsuario">Usuario que la abre.</param>
        /// <returns>Id de la planilla.</returns>
        Task<int> AbrirPlanillaAsync(int idCurso, DateOnly fecha, int idUsuario);

        Task<Asistencia?> GetPorCursoYFechaAsync(int idCurso, DateOnly fecha);

        /// <summary>Lista las planillas de un curso, de la mas reciente a la mas antigua.</summary>
        /// <param name="idCurso">Curso.</param>
        /// <param name="desde">Fecha inicial inclusive; <c>null</c> para no acotar.</param>
        /// <param name="hasta">Fecha final inclusive; <c>null</c> para no acotar.</param>
        Task<List<Asistencia>> ListarPorCursoAsync(int idCurso, DateOnly? desde, DateOnly? hasta);

        /// <param name="idAlumno">Alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        Task<List<VInasistenciaAnual>> ListarInasistenciasAnualesAsync(int idAlumno, int? ciclo);

        /// <param name="idAlumno">Alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        Task<List<VInasistenciaPeriodo>> ListarInasistenciasPorBimestreAsync(int idAlumno, int? ciclo);
    }
}
