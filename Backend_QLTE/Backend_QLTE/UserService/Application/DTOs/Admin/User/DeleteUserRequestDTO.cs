using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.User
{
    public class DeleteUserRequestDTO
    {
        [Required(ErrorMessage = "UserId là bắt buộc")]
        public string UserId { get; set; }
    }
}
