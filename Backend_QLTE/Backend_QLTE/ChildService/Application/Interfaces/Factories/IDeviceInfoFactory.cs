using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Factories
{
    public interface IDeviceInfoFactory
    {
        DeviceInfo Create(DeviceInfoCreateDTO dto);
        DeviceInfo Update(DeviceInfoUpdateDTO dto);
        DeviceInfo Delete(DeviceInfoDeleteDTO dto);
    }
}
