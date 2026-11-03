using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Boletin"/>.</summary>
    public interface IBoletinRepository : IRepository<Boletin>
    {
        /// <param name="idAlumno">Alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        /// <param name="soloPublicados">Si es <c>true</c>, omite los boletines en borrador.</param>
        Task<List<VBoletinResumen>> ListarResumenPorAlumnoAsync(int idAlumno, int? ciclo, bool soloPublicados);

        /// <summary>Lista los resumenes de un curso ordenados por apellido y nombre.</summary>
        /// <param name="idCurso">Curso.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        /// <param name="bimestre">Bimestre (1 a 4); <c>null</c> para todos.</param>
        Task<List<VBoletinResumen>> ListarResumenPorCursoAsync(int idCurso, int? ciclo, short? bimestre);

        Task<VBoletinResumen?> GetResumenAsync(int idBoletin);

        /// <summary>Lista las notas por materia de un boletin, ordenadas por materia.</summary>
        Task<List<VBoletinDetalle>> ListarDetalleAsync(int idBoletin);

        /// <summary>Genera el boletin de un alumno (funcion <c>fn_generar_boletin</c>).</summary>
        /// <param name="idAlumno">Alumno.</param>
        /// <param name="idPeriodo">Periodo bimestral.</param>
        /// <param name="idCreador">Usuario que lo genera.</param>
        /// <returns>Id del boletin.</returns>
        Task<int> GenerarAsync(int idAlumno, int idPeriodo, int idCreador);

        /// <summary>Genera los boletines de todo un curso (funcion <c>fn_generar_boletines_curso</c>).</summary>
        /// <param name="idCurso">Curso.</param>
        /// <param name="idPeriodo">Periodo bimestral.</param>
        /// <param name="idCreador">Usuario que los genera.</param>
        /// <returns>Cantidad de alumnos alcanzados.</returns>
        Task<int> GenerarPorCursoAsync(int idCurso, int idPeriodo, int idCreador);
    }
}
