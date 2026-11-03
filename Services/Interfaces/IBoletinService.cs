using Plataforma24API.DTOs.Boletines;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Generacion, carga de notas y consulta de boletines.</summary>
    public interface IBoletinService
    {
        /// <summary>
        /// Lista los boletines de un alumno. El personal ve todos; el alumno solo los suyos y ya publicados.
        /// </summary>
        /// <param name="idAlumno">Alumno.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        /// <exception cref="Common.Exceptions.ForbiddenException">Un alumno consulto a otro.</exception>
        Task<IEnumerable<BoletinResumenDTO>> ListarPorAlumnoAsync(int idAlumno, int? ciclo);

        /// <param name="idCurso">Curso.</param>
        /// <param name="ciclo">Ciclo lectivo; <c>null</c> para todos.</param>
        /// <param name="bimestre">Bimestre (1 a 4); <c>null</c> para todos.</param>
        Task<IEnumerable<BoletinResumenDTO>> ListarPorCursoAsync(int idCurso, int? ciclo, short? bimestre);

        /// <summary>Devuelve el boletin con la nota de cada materia. Un alumno solo accede a los suyos publicados.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">No existe o aun no fue publicado para el alumno.</exception>
        /// <exception cref="Common.Exceptions.ForbiddenException">Un alumno consulto el boletin de otro.</exception>
        Task<BoletinDetalleDTO> BuscarAsync(int id);

        /// <returns>Id del boletin generado.</returns>
        Task<int> GenerarAsync(GenerarBoletinDTO dto);

        /// <returns>Cantidad de alumnos alcanzados.</returns>
        Task<int> GenerarPorCursoAsync(GenerarBoletinesCursoDTO dto);

        /// <summary>Carga la nota u observacion de una materia y registra quien la cargo.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El boletin no incluye esa materia.</exception>
        Task CargarNotaAsync(int idBoletin, int idMateria, ModificarNotaDTO dto);

        /// <summary>Modifica las observaciones o el estado de publicacion de un boletin.</summary>
        /// <exception cref="Common.Exceptions.NotFoundException">El boletin no existe.</exception>
        Task ActualizarAsync(int id, ModificarBoletinDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El boletin no existe.</exception>
        Task EliminarAsync(int id);
    }
}
