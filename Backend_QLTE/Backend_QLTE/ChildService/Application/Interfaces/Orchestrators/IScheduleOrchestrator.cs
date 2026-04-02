using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Orchestrators
{
    public interface IScheduleOrchestrator
    {
        Task<Schedule> CreateAsync(ScheduleCreateDTO dto, CancellationToken cancellationToken = default);
        Task<Schedule> UpdateAsync(ScheduleUpdateDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(ScheduleDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
