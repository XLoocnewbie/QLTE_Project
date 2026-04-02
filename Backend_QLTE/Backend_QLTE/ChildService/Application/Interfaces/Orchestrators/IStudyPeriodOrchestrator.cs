using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Orchestrators
{
    public interface IStudyPeriodOrchestrator
    {
        Task<StudyPeriod> CreateAsync(StudyPeriodCreateDTO dto, CancellationToken cancellationToken = default);
        Task<StudyPeriod> UpdateAsync(StudyPeriodUpdateDTO dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(StudyPeriodDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
