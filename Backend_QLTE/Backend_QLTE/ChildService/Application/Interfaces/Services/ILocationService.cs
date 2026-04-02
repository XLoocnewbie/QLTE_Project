using Backend_QLTE.ChildService.Application.DTOs.Client.Location;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<ResultDTO> CreateLocationHistoryAsync(CreateLocationHistoryRequestDTO request);
        Task<ResultDTO<LocationHistoryResponseDTO>> GetLocationHistoryNewAsync(Guid childId);
    }
}
