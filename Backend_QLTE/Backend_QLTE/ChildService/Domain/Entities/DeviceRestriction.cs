using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_QLTE.ChildService.Domain.Entities
{
    public class DeviceRestriction
    {
        [Key]
        public Guid RestrictionId { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        [ForeignKey(nameof(DeviceId))]
        public virtual DeviceInfo Device { get; set; } = default!;

        // Danh sách app bị chặn (vd: "facebook, youtube, tiktok")
        [MaxLength(500)]
        public string? BlockedApps { get; set; }

        // Danh sách website bị chặn (vd: "youtube.com, facebook.com")
        [MaxLength(1000)]
        public string? BlockedDomains { get; set; }

        // Danh sách website được phép (nếu muốn dùng AllowList)
        [MaxLength(1000)]
        public string? AllowedDomains { get; set; }

        // Cờ trạng thái firewall
        public bool IsFirewallEnabled { get; set; } = false;

        // Kiểu chế độ: "StudyMode", "Custom", "EmergencyLock"
        [MaxLength(50)]
        public string Mode { get; set; } = "Custom";

        // Thời điểm bật/tắt
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
