using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class StudyPeriod
    {
        [Key]
        public Guid StudyPeriodId { get; set; }

        [Required]
        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [MaxLength(50)]
        public string? MoTa { get; set; }

        public bool IsActive { get; set; } = true;

        // Tuỳ chọn nâng cao (nếu muốn)
        [MaxLength(20)]
        public string RepeatPattern { get; set; } = "Daily"; // Daily / Weekday / Custom

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
