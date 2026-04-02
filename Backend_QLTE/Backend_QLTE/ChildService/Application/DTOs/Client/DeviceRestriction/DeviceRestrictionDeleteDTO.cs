using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction
{
    public class DeviceRestrictionDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id cấu hình hạn chế.")]
        public Guid RestrictionId { get; set; }
    }
}
