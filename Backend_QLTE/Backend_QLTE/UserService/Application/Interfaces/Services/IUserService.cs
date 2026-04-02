using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.DTOs.Common;

namespace Backend_QLTE.UserService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ResultDTO> RegisterUserAsync(UserRegisterDTO registerUser, CancellationToken cancellationToken = default); // Đăng ký User
        Task<ResultDTO<ExternalLoginUserResponseDTO>> ProviderLoginOrRegisterUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default); // Đăng nhập hoặc đăng ký User bằng Google
        Task<ResultDTO<UpdateInfoUserResponseDTO>> UpdateInfoUserAsync(InfoUserUpdateRequestDTO update,CancellationToken cancellationToken = default); // Update thông tin người dùng
        Task<ResultDTO<LoginLocalUserResponseDTO>> LoginLocalUserAsync(LoginRequestDTO requestLogin,CancellationToken cancellationToken = default); // Đăng nhập User
        Task<ResultDTO<UserResponseDTO>> FindUserByEmailAsync(FindUserByEmailRequestDTO dto, CancellationToken cancellationToken = default); // Tìm User theo email
        Task<ResultDTO<UserResponseDTO>> FindUserByUserIdAsync(FindUserByIdRequestDTO dto, CancellationToken cancellationToken = default); // Tìm User theo userID
        Task<ResultListDTO<UserResponseDTO>> FindUserByTenNDAsync(FindUserByTenNDRequestTO dto, CancellationToken cancellationToken = default); // Tìm User theo NameND
        Task<ResultDTO> ResetPasswordAsync(ResetPasswordRequestDTO dto, CancellationToken cancellationToken = default); // Reset mật khẩu
        Task<ResultDTO> ChangeEmailAsync(ChangeEmailRequestDTO dto, CancellationToken cancellationToken = default); // Đổi email
        Task<ResultListDTO<UserResponseDTO>> GetAllUsersAsync(ListUserRequestDTO request, CancellationToken cancellationToken = default); // Lấy danh sách tất cả người dùng
        Task<ResultDTO> DeleteUserAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default); // Xóa User
    }
}
