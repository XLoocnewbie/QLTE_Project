using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Domain.Services.Interfaces
{
    public interface IRoleDomainService
    {
        void EnsureCanCreate(Role role); // Kiểm tra các điều kiện để tạo Role mới  
        void EnsureCanDelete(Role role); // Kiểm tra các điều kiện để xóa Role
        void EnsureCanUpdate(Role role, string roleName); // Kiểm tra các điều kiện để cập nhật Role
        void EnsureCanFindUserName(string roleName, int page, int limit); // Kiểm tra các điều kiện để tìm Role theo tên
        void EnsureCanGetListRole(int page, int limit); // Kiểm tra các điều kiện để lấy danh sách Role với phân trang
    }
}
