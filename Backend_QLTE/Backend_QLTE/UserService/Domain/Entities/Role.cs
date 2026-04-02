using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Backend_QLTE.UserService.Domain.Entities
{
    public class Role : IdentityRole<string>
    {
        public Role() : base() { }
        public Role(string roleName) : base(roleName) { }

        [Display(Name = "Tên vai trò")]
        public string RoleName => Name;

        [Display(Name = "Tên chuẩn hóa")]
        public string NormalizedRoleName => NormalizedName;

        [Display(Name = "Mã đồng bộ")]
        public string SyncStamp => ConcurrencyStamp;

        [Display(Name = "Thời gian tạo")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;

        [Display(Name = "Thời gian cập nhật")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? ThoiGianCapNhat { get; set; }

        public bool DuplicateRoleName(string roleName)
        {
            return string.Equals(NormalizedName, roleName?.ToUpperInvariant(), StringComparison.Ordinal);
        }

    }
}
