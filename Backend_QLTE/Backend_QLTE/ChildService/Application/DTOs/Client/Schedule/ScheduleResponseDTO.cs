using System;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Schedule
{
    public class ScheduleResponseDTO
    {
        public Guid ScheduleId { get; set; }
        public Guid ChildId { get; set; }
        public string TenMonHoc { get; set; } = string.Empty;
        public DayOfWeek Thu { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }
}
