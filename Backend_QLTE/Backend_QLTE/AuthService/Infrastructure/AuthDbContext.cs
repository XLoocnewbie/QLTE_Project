using Backend_QLTE.AuthService.Application.Interfaces.Repositories;
using Backend_QLTE.AuthService.Domain.Entities;
using Backend_QLTE.AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend_QLTE.AuthService.Infrastructure
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        // Bắt đầu một giao dịch mới
        public async Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }


    }
}
