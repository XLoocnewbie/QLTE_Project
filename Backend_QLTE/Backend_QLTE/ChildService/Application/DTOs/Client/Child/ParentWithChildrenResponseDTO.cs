using System;
using System.Collections.Generic;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class ParentWithChildrenResponseDTO
    {
        public string ParentId { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AvatarND { get; set; }
        public List<ChildSummaryDTO> Children { get; set; } = new();
    }
}
