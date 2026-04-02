using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class CreateChildRequestDTO
    {
        [Required(ErrorMessage = "ParentId không được để trống")]
        public string ParentId { get; set; } // Email đăng ký
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } // Email đăng ký

        [RegularExpression(@"^(?:\+84|0)\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; } // Số điện thoại đăng ký

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
            ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự, có chữ hoa, số và ký tự đặc biệt [ @$!%*?&. ]!")]
        public string Password { get; set; } // Mật khẩu đăng ký

        [Required(ErrorMessage = "Mật khẩu nhập lại không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; } // Mật khẩu đăng ký

        [Required(ErrorMessage = "Tên không được để trống")]
        public string FullName { get; set; }

        public IFormFile AvatarND { get; set; }

        public int? GioiTinh { get; set; }
        [Required(ErrorMessage = "Ngay Sinh không được để trống")]
        public DateTime NgaySinh { get; set; }
    }
}
