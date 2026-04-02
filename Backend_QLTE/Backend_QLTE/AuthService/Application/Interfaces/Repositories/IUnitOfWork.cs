namespace Backend_QLTE.AuthService.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IRefreshTokenRepository RefreshTokens { get; }
        Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default); // Transaction
    }
}
