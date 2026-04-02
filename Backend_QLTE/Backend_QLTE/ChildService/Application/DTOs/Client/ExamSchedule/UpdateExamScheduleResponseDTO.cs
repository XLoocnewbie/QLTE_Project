using System;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule
{
    public class UpdateExamScheduleResponseDTO
    {
        public Guid ExamId { get; set; }
        public Guid ChildId { get; set; }
        public string MonThi { get; set; } = string.Empty;
        public DateTime ThoiGianThi { get; set; }
        public string? GhiChu { get; set; }
        public bool DaThiXong { get; set; }
    }
}
