using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class Schedule
    {
        [Key]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public virtual Child Child { get; set; } = default!;

        [Required, MaxLength(100)]
        public string TenMonHoc { get; set; } = string.Empty; // Toán, Anh, Vẽ, Thể dục,...

        [Required]
        public DayOfWeek Thu { get; set; } // Thứ trong tuần (T2 - CN)

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }
    }
}
