using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Domain.Entities;

namespace Backend_QLTE.UserService.Application.Interfaces.Orchestrators
{
    public interface IUserOrchestrator
    {
        Task<User> RegisterUserAsync(UserRegisterDTO registerUser, CancellationToken cancellationToken = default); // Đăng ký User mới với vai trò mặc định
        Task<User> ProviderRegisterUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default); // Đăng ký User mới từ bên ngoài với vai trò mặc định
        User ProviderLoginUser(ExternalLoginUserInfoDTO request); // Đăng nhập User từ bên ngoài
        Task<User> UpdateInfoUserAsync(InfoUserUpdateRequestDTO update, string userId ,CancellationToken cancellationToken = default); // Cập nhật thông tin User
        Task<User> LoginLocalUserAsync(LoginRequestDTO requestLogin, CancellationToken cancellationToken = default); // Đăng nhập User Local
        User GetUserByEmail(FindUserByEmailRequestDTO dto); // Tìm User theo email
        User GetUserByUserId(FindUserByIdRequestDTO dto); // Tìm User theo userId
        Task<(List<User> user, int total, int last)> GetUserByNameND(FindUserByTenNDRequestTO request, CancellationToken cancellationToken = default); // Tìm User theo NameND
        User ResetPasswordAsync(ResetPasswordRequestDTO dto); // Reset mật khẩu User
        User ChangeEmail(User user, string newEmail); // Đổi email User
        Task GetListUserAsync(ListUserRequestDTO request, CancellationToken cancellationToken = default); // Lấy danh sách User với phân trang và lọc
        Task<User> DeleteUserAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default); // Xóa User
    }
} 
