using HotChocolate.Authorization;
using Plataforma24API.DTOs.Cursos;
using Plataforma24API.DTOs.Materias;
using Plataforma24API.DTOs.Tareas;
using Plataforma24API.Models;
using Plataforma24API.Services.Interfaces;

namespace Plataforma24API.GraphQL
{
    /// <summary>
    /// Consultas GraphQL. Es una capa de presentacion igual que los controladores: delega en los servicios
    /// y por lo tanto reutiliza sus reglas de negocio y de autorizacion.
    /// </summary>
    [Authorize]
    public class Query
    {
        /// <summary>Lista los cursos, con filtros opcionales.</summary>
        /// <param name="servicio">Servicio que HotChocolate inyecta en el resolver.</param>
        /// <param name="ano">Anio de cursada.</param>
        /// <param name="turno">Turno.</param>
        public Task<IEnumerable<CursoDTO>> Cursos(ICursoService servicio, Year? ano, Turno? turno)
        {
            return servicio.ListarAsync(ano, turno);
        }

        /// <summary>Lista las materias, con filtros opcionales.</summary>
        /// <param name="servicio">Servicio que HotChocolate inyecta en el resolver.</param>
        /// <param name="idProfesor">Profesor a cargo.</param>
        /// <param name="idCurso">Curso.</param>
        public Task<IEnumerable<MateriaDTO>> Materias(IMateriaService servicio, int? idProfesor, int? idCurso)
        {
            return servicio.ListarAsync(idProfesor, idCurso);
        }

        /// <summary>Lista las tareas, opcionalmente de una materia.</summary>
        /// <param name="servicio">Servicio que HotChocolate inyecta en el resolver.</param>
        /// <param name="idMateria">Materia.</param>
        public Task<IEnumerable<TareaDTO>> Tareas(ITareaService servicio, int? idMateria)
        {
            return servicio.ListarAsync(idMateria);
        }

        /// <summary>Devuelve una tarea por su id.</summary>
        public Task<TareaDTO> Tarea(ITareaService servicio, int id)
        {
            return servicio.BuscarAsync(id);
        }
    }
}
