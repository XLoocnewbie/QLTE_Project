using Backend_QLTE.AuthService.Application.Interfaces.Repositories;
using Backend_QLTE.AuthService.Infrastructure.Data;

namespace Backend_QLTE.AuthService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;

        public UnitOfWork(AuthDbContext context, IRefreshTokenRepository refreshToken)
        {
            _context = context;
            RefreshTokens = refreshToken;
        }

        public IRefreshTokenRepository RefreshTokens { get; }

        public async Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }
    }

}
