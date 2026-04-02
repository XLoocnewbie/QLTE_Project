using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class ExamSchedule
    {
        [Key]
        public Guid ExamId { get; set; }

        [Required]
        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        [Required, MaxLength(100)]
        public string MonThi { get; set; } = string.Empty;

        [Required]
        public DateTime ThoiGianThi { get; set; }

        [MaxLength(255)]
        public string? GhiChu { get; set; }

        public bool DaThiXong { get; set; } = false;

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
}
