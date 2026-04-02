using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.User
{
    public class FindUserByTenNDRequestTO
    {
        [Required(ErrorMessage = "Tên người dùng là bắt bộc!")]
        public string TenND { get; set; }

        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
