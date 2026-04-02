using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Domain.Exceptions.Duplicate;
using Backend_QLTE.UserService.Domain.Exceptions.Invalid;
using Backend_QLTE.UserService.Domain.Services.Interfaces;

namespace Backend_QLTE.UserService.Domain.Services
{
    public class RoleDomainService : IRoleDomainService
    {

        // Kiểm tra các điều kiện để tạo Role mới
        public void EnsureCanCreate(Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                throw new InvalidRoleNameNullException(role.RoleName);
            }
        }

        // Kiểm tra các điều kiện để xóa Role
        public void EnsureCanDelete(Role role)
        {
            // Thêm các kiểm tra khác nếu cần thiết
            if (string.IsNullOrWhiteSpace(role.Id))
            {
                throw new InvalidRoleIdNullException();
            }

        }

        // Kiểm tra các điều kiện để cập nhật Role
        public void EnsureCanUpdate(Role role, string roleName)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
            {
                throw new InvalidRoleNameNullException(role.RoleName);
            }
            if(role.DuplicateRoleName(roleName))
                throw new  DuplicateRoleNameException(roleName);
        }

        // Kiểm tra các điều kiện để tìm Role theo tên
        public void EnsureCanFindUserName(string roleName, int page, int limit)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new InvalidRoleNameNullException(roleName);
            }

            ValidatePagination(page, limit);
        }

        // Kiểm tra các điều kiện để tìm Role theo Id
        public void EnsureCanGetListRole(int page, int limit)
        {
            ValidatePagination(page, limit);
        }

        //private methods
        // Validate phân trang
        private void ValidatePagination(int page, int limit)
        {
            if (page <= 0)
                throw new InvalidUserPageException(page);

            if (limit <= 0 || limit > 100)
                throw new InvalidUserPageLimitException(limit);
        }

    }
}
