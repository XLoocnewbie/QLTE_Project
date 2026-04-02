using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod
{
    public class StudyPeriodDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id của khung giờ học.")]
        public Guid StudyPeriodId { get; set; }
    }
}
