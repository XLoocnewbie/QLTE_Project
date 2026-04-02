using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction
{
    public class DeviceRestrictionCreateDTO
    {
        [Required(ErrorMessage = "Thiếu Id thiết bị.")]
        public Guid DeviceId { get; set; }

        [MaxLength(DtoInvariants.BlockedAppsMaxLength, ErrorMessage = "Danh sách ứng dụng bị chặn không vượt quá {1} ký tự.")]
        public string? BlockedApps { get; set; }

        [MaxLength(DtoInvariants.BlockedDomainsMaxLength, ErrorMessage = "Danh sách website bị chặn không vượt quá {1} ký tự.")]
        public string? BlockedDomains { get; set; }

        [MaxLength(DtoInvariants.AllowedDomainsMaxLength, ErrorMessage = "Danh sách website cho phép không vượt quá {1} ký tự.")]
        public string? AllowedDomains { get; set; }

        public bool IsFirewallEnabled { get; set; } = false;

        [MaxLength(DtoInvariants.ModeMaxLength, ErrorMessage = "Chế độ không vượt quá {1} ký tự.")]
        public string Mode { get; set; } = "Custom";
    }
}
