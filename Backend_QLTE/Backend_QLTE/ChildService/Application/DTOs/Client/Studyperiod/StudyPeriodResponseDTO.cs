using System;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod
{
    public class StudyPeriodResponseDTO
    {
        public Guid StudyPeriodId { get; set; }
        public Guid ChildId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string RepeatPattern { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public bool IsActive { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
