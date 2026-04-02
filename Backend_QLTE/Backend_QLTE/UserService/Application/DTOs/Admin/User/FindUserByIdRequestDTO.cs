using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.User
{
    public class FindUserByIdRequestDTO
    {
        [Required(ErrorMessage = "UserId bắt buộc nhập")]
        public string UserId { get; set; }
    }
}
