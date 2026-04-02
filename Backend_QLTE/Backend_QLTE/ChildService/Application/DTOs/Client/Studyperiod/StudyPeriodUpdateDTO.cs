using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod
{
    public class StudyPeriodUpdateDTO
    {
        [Required(ErrorMessage = "Thiếu Id của khung giờ học.")]
        public Guid StudyPeriodId { get; set; }

        [Required(ErrorMessage = "Thiếu Id của trẻ (ChildId).")]
        public Guid ChildId { get; set; }

        [Required(ErrorMessage = "Thiếu thời gian bắt đầu.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Thiếu thời gian kết thúc.")]
        public TimeSpan EndTime { get; set; }

        [MaxLength(50, ErrorMessage = "Mô tả không vượt quá {1} ký tự.")]
        public string? MoTa { get; set; }

        [MaxLength(20, ErrorMessage = "Thông tin lặp lại không vượt quá {1} ký tự.")]
        public string RepeatPattern { get; set; } = "Daily";
    }
}
