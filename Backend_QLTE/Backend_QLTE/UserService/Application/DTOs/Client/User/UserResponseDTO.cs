namespace Backend_QLTE.UserService.Application.DTOs.Client.User
{
    public class UserResponseDTO
    {
        public string UserId { get; set; } // id người dùng 
        public string UserName { get; set; } // Tên tài khoản
        public string Email { get; set; } // Email
        public string PhoneNumber { get; set; }
        public string NameND { get; set; } // Tên người dùng
        public int GioiTinh { get; set; } // Giới tính
        public string Mota { get; set; } // Mô tả
        public string AvatarND { get; set; } // AvatarND
        public string TypeLogin { get; set; } // Loại đăng nhập
        public string Role { get; set; }
        public DateTime ThoiGianCapNhat { get; set; } // Thời gian cập nhậts
        public DateTime ThoiGianTao { get; set; } // Thời gian tạo tài khoản
    }
}
