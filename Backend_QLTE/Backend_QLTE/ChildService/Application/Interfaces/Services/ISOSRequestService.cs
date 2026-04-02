using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface ISOSRequestService
    {
        Task<ResultListDTO<SOSRequestResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO<SOSRequestResponseDTO>> GetDetailAsync(Guid sosId, CancellationToken cancellationToken = default);
        Task<ResultListDTO<SOSRequestResponseDTO>> GetByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);
        Task<ResultDTO<SOSRequestResponseDTO>> CreateAsync(SOSRequestCreateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO<SOSRequestResponseDTO>> UpdateAsync(SOSRequestUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<ResultDTO> DeleteAsync(SOSRequestDeleteDTO dto, CancellationToken cancellationToken = default);
    }
}
