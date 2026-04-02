using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Infrastructure.Data;

namespace Backend_QLTE.UserService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _context;

        public UnitOfWork(UserDbContext context, IUserRepository users, IRoleRepository roles)
        {
            _context = context;
            Users = users;
            Roles = roles;
        }

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }

        public async Task<IEfTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            return new EfTransaction(transaction);
        }
    }

}
