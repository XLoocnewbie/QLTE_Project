using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class SOSRequestRepository : ISOSRequestRepository
    {
        private readonly ChildDbContext _context;

        public SOSRequestRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách SOSRequest có phân trang
        public async Task<(List<SOSRequest> sosRequests, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<SOSRequest>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var sosRequests = await query
                .OrderByDescending(r => r.ThoiGian)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (sosRequests, total, last);
        }

        // 🟡 Lấy 1 bản ghi theo Id
        public async Task<SOSRequest?> GetByIdAsync(Guid sosId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<SOSRequest>()
                .FirstOrDefaultAsync(r => r.SOSId == sosId, cancellationToken);
        }

        // 🔵 Lấy danh sách yêu cầu SOS của 1 đứa trẻ cụ thể
        public async Task<List<SOSRequest>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<SOSRequest>()
                .Where(r => r.ChildId == childId)
                .OrderByDescending(r => r.ThoiGian)
                .ToListAsync(cancellationToken);
        }

        // 🟣 Lấy danh sách yêu cầu SOS của 1 đứa trẻ có phân trang
        public async Task<(List<SOSRequest> sosRequests, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<SOSRequest>()
                .Where(r => r.ChildId == childId)
                .OrderByDescending(r => r.ThoiGian);

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var sosRequests = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (sosRequests, total, last);
        }

        // 🟢 Tạo mới
        public async Task<SOSRequest> CreateAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default)
        {
            _context.SOSRequests.Add(sosRequest);
            await _context.SaveChangesAsync(cancellationToken);
            return sosRequest;
        }

        // 🟡 Cập nhật
        public async Task<SOSRequest> UpdateAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return sosRequest;
        }

        // 🔴 Xóa
        public async Task DeleteAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default)
        {
            _context.SOSRequests.Remove(sosRequest);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
