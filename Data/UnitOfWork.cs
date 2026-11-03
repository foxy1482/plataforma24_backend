using Microsoft.EntityFrameworkCore.Storage;
using Plataforma24API.Repositories.Interfaces;

namespace Plataforma24API.Data
{
    /// <summary>Unidad de trabajo sobre <see cref="AppDbContext"/>: confirma en una sola operacion los cambios que los repositorios dejaron pendientes.</summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaccion = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaccion);
        }

        private sealed class EfTransaction : IUnitOfWorkTransaction
        {
            private readonly IDbContextTransaction _transaccion;

            public EfTransaction(IDbContextTransaction transaccion)
            {
                _transaccion = transaccion;
            }

            /// <inheritdoc />
            public Task CommitAsync(CancellationToken cancellationToken = default)
            {
                return _transaccion.CommitAsync(cancellationToken);
            }

            /// <inheritdoc />
            public ValueTask DisposeAsync()
            {
                return _transaccion.DisposeAsync();
            }
        }
    }
}
