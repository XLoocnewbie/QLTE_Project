using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Domain.Entities;
using Backend_QLTE.AuthService.Domain.Models;
using Backend_QLTE.AuthService.Domain.ValueObjects;

namespace Backend_QLTE.AuthService.Application.Interfaces.Orchestrators
{
    public interface IAuthOrchestrator
    {
        Task<UserClaims> FetchUserClaimsForLoginAsync(LoginUserPasswordRequestDTO request, CancellationToken cancellationToken = default); // Lấy thông tin user từ dịch vụ UserService khi đăng nhập bằng tài khoản và mật khẩu
        Task<UserClaims> FetchUserClaimsByIdAsync(string userId, CancellationToken cancellationToken = default); // Lấy thông tin user từ dịch vụ UserService theo userId
        Task<UserClaims> FetchUserClaimsFromExternalAsync(ExternalAuthUserInfoDTO request, CancellationToken cancellationToken = default); // Lấy thông tin user từ dịch vụ UserService khi đăng nhập bằng bên thứ 3
        Task<UserResponseDTO> FetchUserByEmailAsync(string email, CancellationToken cancellationToken = default); // Lấy thông tin user từ dịch vụ UserService theo email
        Task<ResultDTO> ChangeEmailAsync(ChangeEmailHttpRequestDTO request, CancellationToken cancellationToken = default); // Đổi email
        Task<ResultDTO> ResetPasswordAsync(ResetPasswordEmailHttpRequestDTO request, CancellationToken cancellationToken = default); // Đặt lại mật khẩu cho user
    }
}
