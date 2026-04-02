using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule
{
    public class ExamScheduleUpdateDTO
    {
        [Required(ErrorMessage = "Thiếu Id của lịch thi (ExamId).")]
        public Guid ExamId { get; set; }

        [Required(ErrorMessage = "Thiếu Id của trẻ (ChildId).")]
        public Guid ChildId { get; set; }

        [Required(ErrorMessage = "Thiếu tên môn thi.")]
        [MaxLength(DtoInvariants.MonThiMaxLength, ErrorMessage = "Tên môn thi không vượt quá {1} ký tự.")]
        public string MonThi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thiếu thời gian thi.")]
        public DateTime ThoiGianThi { get; set; }

        [MaxLength(DtoInvariants.GhiChuMaxLength, ErrorMessage = "Ghi chú không vượt quá {1} ký tự.")]
        public string? GhiChu { get; set; }

        public bool DaThiXong { get; set; } = false;
    }
}
