using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IStudyPeriodRepository
    {
        Task<(List<StudyPeriod> studyPeriods, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<StudyPeriod?> GetByIdAsync(Guid studyPeriodId, CancellationToken cancellationToken = default);
        Task<List<StudyPeriod>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default);
        Task<(List<StudyPeriod> studyPeriods, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<StudyPeriod> CreateAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default);
        Task<StudyPeriod> UpdateAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default);
        Task DeleteAsync(StudyPeriod studyPeriod, CancellationToken cancellationToken = default);
        Task<StudyPeriod?> GetActiveByChildAsync(Guid childId, CancellationToken cancellationToken = default);
    }
}
