namespace Backend_QLTE.AuthService.Application.Interfaces.Repositories
{
    public interface IEfTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken cancellationToken = default); // Commit the transaction
        Task RollbackAsync(CancellationToken cancellationToken = default); // Rollback the transaction
    }
}
