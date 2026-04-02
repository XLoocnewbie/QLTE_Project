using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Factories
{
    public class DeviceInfoFactory : IDeviceInfoFactory
    {
        public DeviceInfo Create(DeviceInfoCreateDTO dto)
        {
            return new DeviceInfo
            {
                DeviceId = Guid.NewGuid(),
                ChildId = dto.ChildId,
                TenThietBi = string.IsNullOrWhiteSpace(dto.TenThietBi)
                    ? "Thiết bị chưa đặt tên"
                    : dto.TenThietBi.Trim(),
                IMEI = dto.IMEI.Trim(),
                Pin = dto.Pin ?? 0,
                TrangThaiOnline = dto.TrangThaiOnline,
                LanCapNhatCuoi = dto.LanCapNhatCuoi == default ? DateTime.Now : dto.LanCapNhatCuoi,
                IsTracking = dto.IsTracking,
                IsLocked = dto.IsLocked
            };
        }

        public DeviceInfo Update(DeviceInfoUpdateDTO dto)
        {
            return new DeviceInfo
            {
                DeviceId = dto.DeviceId,
                ChildId = dto.ChildId,
                TenThietBi = string.IsNullOrWhiteSpace(dto.TenThietBi)
                    ? "Thiết bị chưa đặt tên"
                    : dto.TenThietBi.Trim(),
                IMEI = dto.IMEI.Trim(),
                Pin = dto.Pin ?? 0,
                TrangThaiOnline = dto.TrangThaiOnline,
                LanCapNhatCuoi = dto.LanCapNhatCuoi == default ? DateTime.Now : dto.LanCapNhatCuoi,
                IsTracking = dto.IsTracking,
                IsLocked = dto.IsLocked
            };
        }

        public DeviceInfo Delete(DeviceInfoDeleteDTO dto)
        {
            return new DeviceInfo
            {
                DeviceId = dto.DeviceId
            };
        }
    }
}
