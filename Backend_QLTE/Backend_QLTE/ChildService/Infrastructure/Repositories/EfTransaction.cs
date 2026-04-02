using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class EfTransaction : IEfTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        // Commit the transaction
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        // Rollback the transaction
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }

        // Dispose the transaction
        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }
}
