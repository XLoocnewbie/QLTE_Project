using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Mappers
{
    public interface IDeviceInfoResponseMapper
    {
        DeviceInfoResponseDTO ToDto(DeviceInfo entity);
        List<DeviceInfoResponseDTO> ToDtoList(List<DeviceInfo> entities);

        UpdateDeviceInfoResponseDTO ToUpdateDto(DeviceInfo entity);
        List<UpdateDeviceInfoResponseDTO> ToUpdateDtoList(List<DeviceInfo> entities);
    }
}
