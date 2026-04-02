using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IExamScheduleRepository
    {
        Task<(List<ExamSchedule> examSchedules, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ExamSchedule?> GetByIdAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<List<ExamSchedule>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default);
        Task<(List<ExamSchedule> examSchedules, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<List<ExamSchedule>> GetUpcomingExamsAsync(Guid childId, DateTime fromDate, CancellationToken cancellationToken = default);
        Task<ExamSchedule> CreateAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default);
        Task<ExamSchedule> UpdateAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default);
        Task DeleteAsync(ExamSchedule examSchedule, CancellationToken cancellationToken = default);
    }
}
