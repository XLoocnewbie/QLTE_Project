using System;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo
{
    public class UpdateDeviceInfoResponseDTO
    {
        public Guid DeviceId { get; set; }
        public Guid ChildId { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty;
        public int? Pin { get; set; }
        public bool TrangThaiOnline { get; set; }
        public DateTime LanCapNhatCuoi { get; set; }
        public bool IsTracking { get; set; }
        public bool IsLocked { get; set; }
    }
}
