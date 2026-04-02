using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.ChildService.Application.DTOs.Client.Child
{
    public class UpdateChildRequestDTO
    {
        [Required(ErrorMessage = "ChildrenId là bắt buộc nhập!")]
        public Guid ChildrenId { get; set; } // Tên tài khoản user

        [Required(ErrorMessage = "Tên tài khoản là bắt buộc")]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Tên tài khoản chỉ được chứa chữ và số")]
        public string UserName { get; set; } // Tên tài khoản user

        [StringLength(255, ErrorMessage = "Tên không quá 255 ký tự")]
        public string? NameND { get; set; } // Tên người dùng

        [Range(0, 2, ErrorMessage = "Giới tính phải là 0 (Khác), 1 (Nam) hoặc 2 (Nữ)")]
        public int? GioiTinh { get; set; } // Giới tính

        [MaxLength(500, ErrorMessage = "Mô tả không quá 500 ký tự")]
        public string? MoTa { get; set; } // Mô tả

        public IFormFile? AvatarND { get; set; } // Avatar người dùng

        [RegularExpression(@"^(?:\+84|0)\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; } // Số điện thoại
        public DateTime NgaySinh { get; set; }
        public bool TrangThai { get; set; }
    }
}
