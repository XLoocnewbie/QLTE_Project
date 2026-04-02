using Backend_QLTE.UserService.Application.DTOs.Common;

namespace Backend_QLTE.UserService.Application.DTOs.Admin.Role
{

    public class RoleResponseDTO
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public DateTime? ThoiGianCapNhat { get; set; }
    }
}
