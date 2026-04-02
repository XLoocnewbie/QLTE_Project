
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.User
{
    public class FindUserByEmailRequestDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
    }
}
