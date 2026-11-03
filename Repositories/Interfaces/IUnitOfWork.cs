namespace Plataforma24API.Repositories.Interfaces
{
    /// <summary>
    /// Confirma en una sola operacion los cambios que los repositorios dejaron pendientes.
    /// Los servicios dependen de esta abstraccion y no del contexto de EF Core.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>Guarda los cambios pendientes y devuelve la cantidad de filas afectadas.</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>Inicia una transaccion para agrupar varias confirmaciones. Si no se hace commit, se revierte al liberarla.</summary>
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>Transaccion abierta por <see cref="IUnitOfWork.BeginTransactionAsync"/>.</summary>
    public interface IUnitOfWorkTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
