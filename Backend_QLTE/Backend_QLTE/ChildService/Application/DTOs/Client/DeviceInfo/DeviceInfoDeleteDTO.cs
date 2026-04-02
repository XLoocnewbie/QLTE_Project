using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo
{
    public class DeviceInfoDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id của thiết bị.")]
        public Guid DeviceId { get; set; }
    }
}
