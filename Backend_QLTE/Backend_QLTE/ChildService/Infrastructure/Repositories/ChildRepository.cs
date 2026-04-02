using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class ChildRepository : IChildRepository
    {
        private readonly ChildDbContext _context;

        public ChildRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách tất cả trẻ (phân trang)
        public async Task<(List<Child> children, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Child>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling(total / (double)limit);

            var children = await query
                .OrderBy(c => c.HoTen)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (children, total, last);
        }

        // 🟡 Lấy 1 trẻ theo ChildId
        public async Task<Child?> GetByIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Child>()
                .Include(c => c.DeviceInfo)
                .Include(c => c.SOSRequests)
                .Include(c => c.StudyPeriods)
                .Include(c => c.Alerts)
                .FirstOrDefaultAsync(c => c.ChildId == childId, cancellationToken);
        }

        // 🔵 Lấy danh sách trẻ theo phụ huynh (UserId)
        public async Task<List<Child>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Child>()
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.HoTen)
                .ToListAsync(cancellationToken);
        }

        // 🟢 Tạo mới
        public async Task<Child> CreateAsync(Child child, CancellationToken cancellationToken = default)
        {
            _context.Children.Add(child);
            await _context.SaveChangesAsync(cancellationToken);
            return child;
        }

        // 🟡 Cập nhật
        public async Task<Child> UpdateAsync(Child child, CancellationToken cancellationToken = default)
        {
            _context.Children.Update(child);
            await _context.SaveChangesAsync(cancellationToken);
            return child;
        }

        // 🔴 Xoá
        public async Task DeleteAsync(Child child, CancellationToken cancellationToken = default)
        {
            _context.Children.Remove(child);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
