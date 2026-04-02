using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo
{
    public class DeviceInfoCreateDTO
    {
        [Required(ErrorMessage = "Thiếu Id của trẻ (ChildId).")]
        public Guid ChildId { get; set; }

        [Required(ErrorMessage = "Thiếu tên thiết bị.")]
        [MaxLength(DtoInvariants.TenThietBiMaxLength, ErrorMessage = "Tên thiết bị không vượt quá {1} ký tự.")]
        public string TenThietBi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thiếu IMEI.")]
        [MaxLength(DtoInvariants.ImeiMaxLength, ErrorMessage = "IMEI không vượt quá {1} ký tự.")]
        public string IMEI { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Mức pin phải nằm trong khoảng 0–100%.")]
        public int? Pin { get; set; }

        public bool TrangThaiOnline { get; set; } = false;

        [Required(ErrorMessage = "Thiếu thời gian cập nhật cuối.")]
        public DateTime LanCapNhatCuoi { get; set; } = DateTime.Now;

        public bool IsTracking { get; set; } = false;
        public bool IsLocked { get; set; } = false;
    }

}
