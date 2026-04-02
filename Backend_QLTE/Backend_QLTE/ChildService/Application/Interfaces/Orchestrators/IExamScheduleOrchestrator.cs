using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Orchestrators
{
    public interface IExamScheduleOrchestrator
    {
        Task<ExamSchedule> CreateAsync(ExamScheduleCreateDTO dto, CancellationToken cancellationToken = default);
        Task<ExamSchedule> UpdateAsync(ExamScheduleUpdateDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(ExamScheduleDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
