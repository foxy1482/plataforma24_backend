using Plataforma24API.DTOs.Cursos;
using Plataforma24API.Models;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de cursos.</summary>
    public interface ICursoService
    {
        /// <param name="ano">Ano de cursada; <c>null</c> para no filtrar.</param>
        /// <param name="turno">Turno; <c>null</c> para no filtrar.</param>
        Task<IEnumerable<CursoDTO>> ListarAsync(Year? ano, Turno? turno);

        /// <exception cref="Common.Exceptions.NotFoundException">El curso no existe.</exception>
        Task<CursoDTO> BuscarAsync(int id);

        /// <exception cref="Common.Exceptions.BadRequestException">Ya existe un curso con ese ano, division, turno y ciclo.</exception>
        Task<CursoDTO> CrearAsync(CursoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El curso no existe.</exception>
        Task ActualizarAsync(int id, ModificarCursoDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">El curso no existe.</exception>
        Task EliminarAsync(int id);

        /// <summary>Asigna al curso los alumnos indicados, que deben existir y no tener curso. Todo o nada.</summary>
        /// <returns>Cantidad de alumnos asignados.</returns>
        /// <exception cref="Common.Exceptions.NotFoundException">El curso no existe.</exception>
        /// <exception cref="Common.Exceptions.BadRequestException">Lista invalida, usuario inexistente, que no es alumno o que ya tiene curso.</exception>
        Task<int> AsignarUsuariosAsync(int idCurso, IReadOnlyCollection<int> idUsuarios);
    }
}
