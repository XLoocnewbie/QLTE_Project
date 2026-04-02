using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Schedule
{
    public class ScheduleCreateDTO
    {
        [Required(ErrorMessage = "Thiếu Id của trẻ (ChildId).")]
        public Guid ChildId { get; set; }

        [Required(ErrorMessage = "Thiếu tên môn học.")]
        [MaxLength(DtoInvariants.TenMonHocMaxLength, ErrorMessage = "Tên môn học không vượt quá {1} ký tự.")]
        public string TenMonHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thiếu thông tin thứ trong tuần.")]
        public DayOfWeek Thu { get; set; }

        [Required(ErrorMessage = "Thiếu giờ bắt đầu.")]
        public TimeSpan GioBatDau { get; set; }

        [Required(ErrorMessage = "Thiếu giờ kết thúc.")]
        public TimeSpan GioKetThuc { get; set; }
    }
}
