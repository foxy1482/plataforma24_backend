using Plataforma24API.Models;

namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>Contrato de persistencia de <see cref="ForoTema"/>.</summary>
    public interface IForoTemaRepository : IRepository<ForoTema>
    {
        /// <summary>Lista los temas visibles: primero los fijados y luego los de actividad mas reciente.</summary>
        /// <param name="idCurso">Limita a los temas de ese curso; <c>null</c> para no filtrar.</param>
        /// <param name="soloCursoYGeneral">Si es <c>true</c>, deja solo los temas generales y los de <paramref name="miCurso"/>.</param>
        /// <param name="miCurso">Curso del usuario; solo se usa si <paramref name="soloCursoYGeneral"/> es <c>true</c>.</param>
        Task<List<VForoTema>> ListarVistaAsync(int? idCurso, bool soloCursoYGeneral, int? miCurso);
    }
}
