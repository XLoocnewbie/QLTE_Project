using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;


namespace Backend_QLTE.UserService.Infrastructure.Data
{
    public class UserDbContext : IdentityDbContext < User,Role,string>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
        }

        // Bắt đầu một giao dịch mới
        public async Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

    }
}
