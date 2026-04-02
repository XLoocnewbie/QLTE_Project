using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule
{
    public class ExamScheduleDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id của lịch thi (ExamId).")]
        public Guid ExamId { get; set; }
    }
}
