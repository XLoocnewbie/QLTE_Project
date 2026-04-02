using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class Child
    {
        [Key]
        public Guid ChildId { get; set; }

        [Required, MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public DateTime NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? AnhDaiDien { get; set; }
        public string? NhomTuoi { get; set; }
        public string? TrangThai { get; set; }

        // 🔗 FK logic (Id của phụ huynh)
        [Required]
        public string UserId { get; set; } = string.Empty;

        // 🧭 Navigation nội bộ
        public ICollection<LocationHistory> LocationHistories { get; set; } = new List<LocationHistory>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
        public ICollection<SOSRequest> SOSRequests { get; set; } = new List<SOSRequest>();
        public ICollection<StudyPeriod> StudyPeriods { get; set; } = new List<StudyPeriod>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

        public DeviceInfo? DeviceInfo { get; set; }
    }
}
