using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IScheduleRepository
    {
        Task<(List<Schedule> schedules, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
        Task<List<Schedule>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default);
        Task<(List<Schedule> schedules, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<Schedule> CreateAsync(Schedule schedule, CancellationToken cancellationToken = default);
        Task<Schedule> UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default);
        Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default);
    }
}
