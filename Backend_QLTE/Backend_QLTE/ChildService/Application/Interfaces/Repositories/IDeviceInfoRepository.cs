using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IDeviceInfoRepository
    {
        Task<(List<DeviceInfo> devices, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);
        Task<DeviceInfo?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
        Task<DeviceInfo?> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default);
        Task<DeviceInfo?> GetByIMEIAsync(string imei, CancellationToken cancellationToken = default);
        Task<bool> ExistsByIMEIAsync(string imei, CancellationToken cancellationToken = default);
        Task<DeviceInfo> CreateAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default);
        Task<DeviceInfo> UpdateAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default);
        Task DeleteAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default);
        Task<List<DeviceInfo>> GetTrackingDevicesAsync(CancellationToken cancellationToken = default);
        Task<bool> SetLockStateAsync(Guid childId, bool isLocked, CancellationToken cancellationToken = default);
    }
}
