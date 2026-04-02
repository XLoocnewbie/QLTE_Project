using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction
{
    public class DeviceRestrictionUpdateDTO
    {
        [Required(ErrorMessage = "Thiếu Id cấu hình hạn chế.")]
        public Guid RestrictionId { get; set; }

        [Required(ErrorMessage = "Thiếu Id thiết bị.")]
        public Guid DeviceId { get; set; }

        [MaxLength(DtoInvariants.BlockedAppsMaxLength)]
        public string? BlockedApps { get; set; }

        [MaxLength(DtoInvariants.BlockedDomainsMaxLength)]
        public string? BlockedDomains { get; set; }

        [MaxLength(DtoInvariants.AllowedDomainsMaxLength)]
        public string? AllowedDomains { get; set; }

        public bool IsFirewallEnabled { get; set; }

        [MaxLength(DtoInvariants.ModeMaxLength)]
        public string Mode { get; set; } = "Custom";

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
