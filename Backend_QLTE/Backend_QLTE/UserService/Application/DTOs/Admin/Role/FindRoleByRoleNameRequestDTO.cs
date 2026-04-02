using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.Role
{
    public class FindRoleByRoleNameRequestDTO
    {
        [Required(ErrorMessage = "RoleName là bắt buộc nhập!")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Page là bắt buộc nhập!")]
        public int Page { get; set; }

        [Required(ErrorMessage = "Limit là bắt buộc nhập!")]
        public int Limit { get; set; }

    }
}
