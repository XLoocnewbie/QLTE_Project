using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Mappers
{
    public class DeviceInfoResponseMapper : IDeviceInfoResponseMapper
    {
        // 🟢 Entity → DTO cơ bản (GET)
        public DeviceInfoResponseDTO ToDto(DeviceInfo entity)
        {
            return new DeviceInfoResponseDTO
            {
                DeviceId = entity.DeviceId,
                ChildId = entity.ChildId,
                TenThietBi = entity.TenThietBi ?? string.Empty,
                IMEI = entity.IMEI ?? string.Empty,
                Pin = entity.Pin,
                TrangThaiOnline = entity.TrangThaiOnline,
                LanCapNhatCuoi = entity.LanCapNhatCuoi,
                IsTracking = entity.IsTracking,
                IsLocked = entity.IsLocked
            };
        }

        // 🟢 Danh sách Entity → Danh sách DTO
        public List<DeviceInfoResponseDTO> ToDtoList(List<DeviceInfo> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        // 🟡 Entity → DTO sau Update
        public UpdateDeviceInfoResponseDTO ToUpdateDto(DeviceInfo entity)
        {
            return new UpdateDeviceInfoResponseDTO
            {
                DeviceId = entity.DeviceId,
                ChildId = entity.ChildId,
                TenThietBi = entity.TenThietBi ?? string.Empty,
                IMEI = entity.IMEI ?? string.Empty,
                Pin = entity.Pin,
                TrangThaiOnline = entity.TrangThaiOnline,
                LanCapNhatCuoi = entity.LanCapNhatCuoi,
                IsTracking = entity.IsTracking,
                IsLocked = entity.IsLocked
            };
        }

        // 🟡 Danh sách Entity → danh sách DTO Update
        public List<UpdateDeviceInfoResponseDTO> ToUpdateDtoList(List<DeviceInfo> entities)
        {
            return entities.Select(ToUpdateDto).ToList();
        }
    }
}
