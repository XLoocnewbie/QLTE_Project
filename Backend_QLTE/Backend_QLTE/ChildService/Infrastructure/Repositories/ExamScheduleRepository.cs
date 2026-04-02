using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class ExamScheduleRepository : IExamScheduleRepository
    {
        private readonly ChildDbContext _context;

        public ExamScheduleRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách lịch thi có phân trang
        public async Task<(List<ExamSchedule> examSchedules, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ExamSchedule>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var examSchedules = await query
                .OrderBy(e => e.ThoiGianThi)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (examSchedules, total, last);
        }

        // 🟡 Lấy 1 lịch thi theo Id
        public async Task<ExamSchedule?> GetByIdAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ExamSchedule>()
                .FirstOrDefaultAsync(e => e.ExamId == examId, cancellationToken);
        }

        // 🔵 Lấy danh sách lịch thi của 1 đứa trẻ cụ thể
        public async Task<List<ExamSchedule>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ExamSchedule>()
                .Where(e => e.ChildId == childId)
                .OrderBy(e => e.ThoiGianThi)
                .ToListAsync(cancellationToken);
        }

        // 🟣 Lấy danh sách lịch thi của 1 đứa trẻ có phân trang
        public async Task<(List<ExamSchedule> examSchedules, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<ExamSchedule>()
                .Where(e => e.ChildId == childId)
                .OrderBy(e => e.ThoiGianThi);

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var examSchedules = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (examSchedules, total, last);
        }

        // 🕓 Lấy danh sách lịch thi sắp tới (ví dụ trong tương lai từ 1 ngày trở đi)
        public async Task<List<ExamSchedule>> GetUpcomingExamsAsync(Guid childId, DateTime fromDate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<ExamSchedule>()
                .Where(e => e.ChildId == childId && e.ThoiGianThi >= fromDate)
                .OrderBy(e => e.ThoiGianThi)
                .ToListAsync(cancellationToken);
        }

        // 🟢 Tạo mới lịch thi
        public async Task<ExamSchedule> CreateAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default)
        {
            _context.ExamSchedules.Add(examSchedule);
            await _context.SaveChangesAsync(cancellationToken);
            return examSchedule;
        }

        // 🟡 Cập nhật lịch thi
        public async Task<ExamSchedule> UpdateAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default)
        {
            _context.ExamSchedules.Update(examSchedule);
            await _context.SaveChangesAsync(cancellationToken);
            return examSchedule;
        }

        // 🔴 Xoá lịch thi
        public async Task DeleteAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default)
        {
            _context.ExamSchedules.Remove(examSchedule);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
