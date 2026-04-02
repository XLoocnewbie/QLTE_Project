using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest
{
    public class SOSRequestDeleteDTO
    {
        [Required(ErrorMessage = "Thiếu Id của yêu cầu SOS.")]
        public Guid SOSId { get; set; }
    }
}
