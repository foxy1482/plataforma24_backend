using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="AsistenciaDetalle"/>.</summary>
    public interface IAsistenciaDetalleRepository : IRepository<AsistenciaDetalle>
    {
        /// <summary>Devuelve el detalle de una planilla con los datos de cada alumno, ordenado por apellido y nombre.</summary>
        /// <param name="idAsistencia">Planilla de asistencia.</param>
        /// <returns>Una tupla por alumno; <c>Datos</c> es <c>null</c> si todavia no cargo sus datos personales.</returns>
        Task<List<(AsistenciaDetalle Detalle, Usuario Alumno, DatosPersonales? Datos)>> ListarConAlumnoAsync(int idAsistencia);

        /// <summary>Devuelve, rastreados, los detalles de los alumnos indicados para poder modificarlos.</summary>
        /// <param name="idAsistencia">Planilla de asistencia.</param>
        /// <param name="idsAlumnos">Alumnos buscados.</param>
        Task<List<AsistenciaDetalle>> ListarPorAlumnosAsync(int idAsistencia, IEnumerable<int> idsAlumnos);
    }
}
