using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Orchestrators
{
    public interface IDeviceInfoOrchestrator
    {
        Task<DeviceInfo> CreateAsync(DeviceInfoCreateDTO dto, string userId, CancellationToken cancellationToken = default);
        Task<DeviceInfo> UpdateAsync(DeviceInfoUpdateDTO dto, string userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(DeviceInfoDeleteDTO dto, string userId, CancellationToken cancellationToken = default);
        Task<bool> SetTrackingStateAsync(Guid childId, bool isTracking, string userId, CancellationToken cancellationToken = default);
        Task<bool> SetLockStateAsync(Guid childId, bool isLocked, string userId, CancellationToken cancellationToken = default);
    }
}
