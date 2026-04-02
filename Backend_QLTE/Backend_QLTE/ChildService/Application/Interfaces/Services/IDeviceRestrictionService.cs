using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IDeviceRestrictionService
    {
        Task<ResultListDTO<DeviceRestrictionResponseDTO>> GetByDeviceIdAsync(Guid deviceId);
        Task<ResultDTO<DeviceRestrictionResponseDTO>> GetDetailAsync(Guid restrictionId);
        Task<ResultDTO<DeviceRestrictionResponseDTO>> CreateAsync(DeviceRestrictionCreateDTO dto);
        Task<ResultDTO<DeviceRestrictionUpdateResponseDTO>> UpdateAsync(DeviceRestrictionUpdateDTO dto);
        Task<ResultDTO> DeleteAsync(DeviceRestrictionDeleteDTO dto);
        Task<ResultDTO<DeviceRestrictionUpdateResponseDTO>> ToggleFirewallAsync(Guid restrictionId);
        Task<ResultDTO> ActivateStudyRestrictionAsync(Guid deviceId);
        Task<ResultDTO> DeactivateRestrictionAsync(Guid deviceId);

    }
}
