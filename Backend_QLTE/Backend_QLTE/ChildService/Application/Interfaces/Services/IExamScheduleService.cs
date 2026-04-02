using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IExamScheduleService
    {
        Task<ResultListDTO<ExamScheduleResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO<ExamScheduleResponseDTO>> GetDetailAsync(Guid examId, CancellationToken cancellationToken = default);
        Task<ResultListDTO<ExamScheduleResponseDTO>> GetAllByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO> CreateAsync(ExamScheduleCreateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO<ExamScheduleResponseDTO>> UpdateAsync(ExamScheduleUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO> DeleteAsync(ExamScheduleDeleteDTO dto, CancellationToken cancellationToken = default);
        Task<ResultListDTO<ExamScheduleResponseDTO>> GetUpcomingExamsAsync(Guid childId, CancellationToken cancellationToken = default);
    }
}
