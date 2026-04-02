using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class StudyPeriodRepository : IStudyPeriodRepository
    {
        private readonly ChildDbContext _context;

        public StudyPeriodRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách StudyPeriod có phân trang
        public async Task<(List<StudyPeriod> studyPeriods, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<StudyPeriod>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var studyPeriods = await query
                .OrderBy(p => p.StudyPeriodId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (studyPeriods, total, last);
        }

        // 🟡 Lấy 1 bản ghi theo Id
        public async Task<StudyPeriod?> GetByIdAsync(Guid studyPeriodId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<StudyPeriod>()
                .FirstOrDefaultAsync(sp => sp.StudyPeriodId == studyPeriodId, cancellationToken);
        }

        // 🔵 Lấy danh sách StudyPeriod của 1 đứa trẻ cụ thể
        public async Task<List<StudyPeriod>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<StudyPeriod>()
                .Where(sp => sp.ChildId == childId)
                .OrderBy(sp => sp.StartTime)
                .ToListAsync(cancellationToken);
        }

        // 🟢 Lấy danh sách StudyPeriod theo ChildId có phân trang
        public async Task<(List<StudyPeriod> studyPeriods, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<StudyPeriod>()
                .Where(sp => sp.ChildId == childId)
                .OrderBy(sp => sp.StartTime);

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var studyPeriods = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (studyPeriods, total, last);
        }

        // 🟢 Tạo mới
        public async Task<StudyPeriod> CreateAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default)
        {
            _context.StudyPeriods.Add(studyPeriod);
            await _context.SaveChangesAsync(cancellationToken);
            return studyPeriod;
        }

        // 🟡 Cập nhật
        public async Task<StudyPeriod> UpdateAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default)
        {
            _context.StudyPeriods.Update(studyPeriod);
            await _context.SaveChangesAsync(cancellationToken);
            return studyPeriod;
        }

        // 🔴 Xóa
        public async Task DeleteAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default)
        {
            _context.StudyPeriods.Remove(studyPeriod);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // 🟣 Lấy khung giờ học đang hoạt động hiện tại của 1 đứa trẻ
        public async Task<StudyPeriod?> GetActiveByChildAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now.TimeOfDay;

            return await _context.Set<StudyPeriod>()
                .Where(sp => sp.ChildId == childId
                             && sp.IsActive
                             && sp.StartTime <= now
                             && sp.EndTime >= now)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
