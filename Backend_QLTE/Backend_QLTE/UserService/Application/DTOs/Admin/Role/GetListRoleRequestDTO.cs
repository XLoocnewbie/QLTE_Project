using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.Role
{
    public class GetListRoleRequestDTO
    {
        [Required(ErrorMessage = "Page là bắt buộc")]
        public int Page { get; set; } = 1;
        [Required(ErrorMessage = "Limit là bắt buộc")]
        public int Limit { get; set; } = 10;
    }
}
