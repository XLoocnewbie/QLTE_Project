namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction
{
    public class DeviceRestrictionUpdateResponseDTO
    {
        public Guid RestrictionId { get; set; }
        public Guid DeviceId { get; set; }

        // Chuỗi CSV hoặc JSON tuỳ cách bạn đang lưu
        public string? BlockedApps { get; set; }
        public string? BlockedDomains { get; set; }
        public string? AllowedDomains { get; set; }

        public bool IsFirewallEnabled { get; set; }
        public string Mode { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}
