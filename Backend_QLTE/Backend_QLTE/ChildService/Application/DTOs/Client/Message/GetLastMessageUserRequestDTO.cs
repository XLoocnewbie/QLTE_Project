using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Message
{
    public class GetLastMessageUserRequestDTO
    {
        [Required(ErrorMessage = "UserId là bắt Buộc")]
        public string UserId { get; set; }
    }
}
