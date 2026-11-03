using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="Materia"/>.</summary>
    public interface IMateriaRepository : IRepository<Materia>
    {
        /// <param name="idProfesor">Profesor a cargo; <c>null</c> o <c>0</c> para no filtrar.</param>
        /// <param name="idCurso">Curso; <c>null</c> o <c>0</c> para no filtrar.</param>
        Task<List<Materia>> ListarAsync(int? idProfesor, int? idCurso);

        /// <summary>Indica si el curso ya tiene una materia con ese nombre.</summary>
        Task<bool> ExisteAsync(int idCurso, string nombre);
    }
}
