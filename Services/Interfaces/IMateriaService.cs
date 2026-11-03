using Plataforma24API.DTOs.Materias;

namespace Plataforma24API.Services.Interfaces
{
    /// <summary>Administracion de materias.</summary>
    public interface IMateriaService
    {
        /// <param name="idProfesor">Profesor a cargo; <c>null</c> o <c>0</c> para no filtrar.</param>
        /// <param name="idCurso">Curso; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<IEnumerable<MateriaDTO>> ListarAsync(int? idProfesor, int? idCurso);

        /// <exception cref="Common.Exceptions.NotFoundException">La materia no existe.</exception>
        Task<MateriaDTO> BuscarAsync(int id);

        /// <exception cref="Common.Exceptions.BadRequestException">El curso ya tiene una materia con ese nombre.</exception>
        Task<MateriaDTO> CrearAsync(MateriaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La materia no existe.</exception>
        Task ActualizarAsync(int id, ModificarMateriaDTO dto);

        /// <exception cref="Common.Exceptions.NotFoundException">La materia no existe.</exception>
        Task EliminarAsync(int id);
    }
}
