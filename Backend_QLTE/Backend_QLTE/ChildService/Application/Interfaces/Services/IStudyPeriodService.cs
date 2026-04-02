using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IStudyPeriodService
    {
        Task<ResultListDTO<StudyPeriodResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO<StudyPeriodResponseDTO>> GetDetailAsync(Guid studyPeriodId, CancellationToken cancellationToken = default);
        Task<ResultListDTO<StudyPeriodResponseDTO>> GetAllStudyPeriodsByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO> CreateAsync(StudyPeriodCreateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO<StudyPeriodResponseDTO>> UpdateAsync(StudyPeriodUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO> DeleteAsync(StudyPeriodDeleteDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO> ToggleActiveAsync(Guid studyPeriodId, CancellationToken cancellationToken = default);
        Task<ResultDTO<StudyPeriodResponseDTO>> GetActiveByChildAsync(Guid childId, CancellationToken cancellationToken = default);
    }
}
