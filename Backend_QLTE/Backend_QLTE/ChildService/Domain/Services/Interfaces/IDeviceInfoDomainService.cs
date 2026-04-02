using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Domain.Services.Interfaces
{
    public interface IDeviceInfoDomainService
    {
        void EnsureCanCreate(DeviceInfo deviceInfo);
        void EnsureCanUpdate(DeviceInfo deviceInfo);
        void EnsureCanDelete(DeviceInfo deviceInfo);
    }
}
