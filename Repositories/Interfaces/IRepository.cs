namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>
    /// Operaciones de persistencia comunes a todas las entidades. Los cambios no se guardan hasta que se
    /// llama a <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    /// <typeparam name="T">Entidad persistida.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Busca por clave primaria. La entidad devuelta queda rastreada para poder modificarla o eliminarla.</summary>
        /// <returns>La entidad, o <c>null</c> si no existe.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Devuelve todas las entidades, sin rastreo (solo lectura).</summary>
        Task<List<T>> GetAllAsync();

        Task AddAsync(T entity);

        void Remove(T entity);
    }
}
