using Microsoft.EntityFrameworkCore;
using Plataforma24API.Data;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Repositories
{
    /// <summary>Repositorio generico con las operaciones comunes. Las consultas de lectura no rastrean entidades; <see cref="GetByIdAsync"/> si, porque su resultado suele modificarse o eliminarse.</summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        /// <inheritdoc />
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        /// <inheritdoc />
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        /// <inheritdoc />
        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}
