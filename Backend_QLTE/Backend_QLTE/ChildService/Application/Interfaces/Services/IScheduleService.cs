using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IScheduleService
    {
        Task<ResultListDTO<ScheduleResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO<ScheduleResponseDTO>> GetDetailAsync(Guid scheduleId, CancellationToken cancellationToken = default);
        Task<ResultListDTO<ScheduleResponseDTO>> GetAllByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO> CreateAsync(ScheduleCreateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO<ScheduleResponseDTO>> UpdateAsync(ScheduleUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO> DeleteAsync(ScheduleDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
