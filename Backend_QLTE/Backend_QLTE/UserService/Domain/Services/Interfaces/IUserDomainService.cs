using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Domain.Services.Interfaces
{
    public interface IUserDomainService
    {
        void EnsureCanRegister(User user); // Kiểm tra điều kiện đăng ký User mới
        void EnsureCanProviderRegister(User user); // Kiểm tra điều kiện đăng ký User mới từ bên ngoài
        void EnsureCanProviderLogin(User user); // Kiểm tra điều kiện đăng nhập User từ bên ngoài
        void EnsureCanLoginLocal(User user); // Kiểm tra điều kiện đăng nhập User Local
        void EnsureCanUpdateInfoUser(User user); // Kiểm tra điều kiện cập nhật thông tin User
        void EnsureCanUserEmail(User user); // Kiểm tra điều kiện email User
        void EnsureCanUserId(User user); // Kiểm tra điều kiện userId User
        void EnsureCanNameND(string tenND, int page, int limit); // Kiểm tra điều kiện tìm kiếm NameND
        void EnsureCanResetPassword(User user); // Kiểm tra điều kiện reset mật khẩu User
        void EnsureCanChangeEmail(User user, string newEmail); // Kiểm tra điều kiện thay đổi email
        void EnsureCanGetListUser(int page, int limit); // Kiểm tra điều kiện lấy danh sách User
    }
}
