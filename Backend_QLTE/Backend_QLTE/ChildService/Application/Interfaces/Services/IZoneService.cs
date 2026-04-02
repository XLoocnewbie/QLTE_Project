using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IZoneService
    {
        Task<ResultListDTO<SafeZoneResponseDTO>> GetSafeZoneByUserIdAsync(string userId);
        Task<ResultListDTO<SafeZoneResponseDTO>> GetSafeZoneByUserIdAndChildIdAsync(string userId, Guid childId);
        Task<ResultDTO> CreateSafeZoneAsync(CreateSafeZoneRequestDTO request);
        Task<ResultDTO<SafeZoneResponseDTO>> UpdateSafeZoneAsync(UpdateSafeZoneRequestDTO request);
        Task<ResultDTO> DeleteSafeZoneByChildrenIdAsync(Guid childrenId);
        Task<ResultDTO> DeleteSafeZoneAsync(Guid safeZoneId);
        Task<ResultListDTO<DangerZoneResponseDTO>> GetDangerZoneByUserIdAsync(string userId);
        Task<ResultListDTO<DangerZoneResponseDTO>> GetDangerZoneByUserIdAndChildIdAsync(string userId, Guid childId);
        Task<ResultDTO> CreateDangerZoneAsync(CreateDangerZoneRequestDTO request);
        Task<ResultDTO<DangerZoneResponseDTO>> UpdateDangerZoneAsync(UpdateDangerZoneRequestDTO request);
        Task<ResultDTO> DeleteDangerZoneByChildrenIdAsync(Guid childrenId);
        Task<ResultDTO> DeleteDangerZoneAsync(Guid dangerZoneId);
        Task<bool> CheckSafeZoneAsync(Guid childId, double lat, double lng);
    }
}
