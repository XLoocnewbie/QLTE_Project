using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Schedule
{
    public class ScheduleDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id của lịch học.")]
        public Guid ScheduleId { get; set; }
    }
}
