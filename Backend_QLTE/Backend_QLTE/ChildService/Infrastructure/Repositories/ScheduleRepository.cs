using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ChildDbContext _context;

        public ScheduleRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách Schedule có phân trang
        public async Task<(List<Schedule> schedules, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Schedule>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var schedules = await query
                .OrderBy(s => s.Thu)
                .ThenBy(s => s.GioBatDau)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (schedules, total, last);
        }

        // 🟡 Lấy 1 bản ghi theo Id
        public async Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Schedule>()
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId, cancellationToken);
        }

        // 🔵 Lấy danh sách lịch học của 1 trẻ
        public async Task<List<Schedule>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Schedule>()
                .Where(s => s.ChildId == childId)
                .OrderBy(s => s.Thu)
                .ThenBy(s => s.GioBatDau)
                .ToListAsync(cancellationToken);
        }

        // 🟢 Lấy danh sách lịch học có phân trang theo ChildId
        public async Task<(List<Schedule> schedules, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Schedule>()
                .Where(s => s.ChildId == childId)
                .OrderBy(s => s.Thu)
                .ThenBy(s => s.GioBatDau);

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var schedules = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (schedules, total, last);
        }

        // 🟢 Tạo mới
        public async Task<Schedule> CreateAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync(cancellationToken);
            return schedule;
        }

        // 🟡 Cập nhật
        public async Task<Schedule> UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            // 🔍 Lấy entity hiện tại đang được EF tracking
            var existing = await _context.Schedules
                .FirstOrDefaultAsync(x => x.ScheduleId == schedule.ScheduleId, cancellationToken);

            if (existing == null)
                throw new InvalidOperationException($"Không tìm thấy Schedule Id={schedule.ScheduleId}");

            // ✅ Cập nhật từng trường cần thiết
            existing.TenMonHoc = schedule.TenMonHoc;
            existing.Thu = schedule.Thu;
            existing.GioBatDau = schedule.GioBatDau;
            existing.GioKetThuc = schedule.GioKetThuc;
            existing.ChildId = schedule.ChildId;

            // ⏰ Lưu thay đổi
            await _context.SaveChangesAsync(cancellationToken);

            return existing;
        }

        // 🔴 Xóa
        public async Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default)
        {
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
