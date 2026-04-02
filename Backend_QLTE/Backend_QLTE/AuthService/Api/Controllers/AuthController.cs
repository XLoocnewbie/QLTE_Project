using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.AuthService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // Login cho token
        [HttpPost("LoginUserToken")]
        public async Task<IActionResult> LoginUserToken(LoginUserPasswordRequestDTO request, CancellationToken cancellationToken = default)
        {   
            _logger.LogInformation("API Response: Bắt đầu LoginUserToken gọi với: {Account}", request.Account);
            var result = await _authService.LoginUserGenerateTokenAsync(request,cancellationToken);
            _logger.LogInformation("API Response: LoginUserToken cho email {Email} với trạng thái {Status}", request.Account, result.Status);
            return Ok(result);
        }

        // Người dùng đăng nhập bằng nhà cung cấp bên ngoài Cho token 'Google,FaceBook,...'  
        [HttpPost("ExternalProviderLoginUserToken")]
        public async Task<IActionResult> ExternalProviderLoginUserToken (ExternalAuthTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu ExternalProviderLoginUserToken gọi với: {Provider}", request.Provider);
            var result = await _authService.LoginExternalGenerateTokenAsync(request,cancellationToken);
            _logger.LogInformation("API Response: ExternalProviderLoginUserToken cho Provider {Provider} với trạng thái {Status}", request.Provider, result.Status);
            return Ok(result);
        }

        // Tạo mới token từ refresh token
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu RefreshToken gọi với: {RefreshToken}", request.RefreshToken);
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);
            _logger.LogInformation("API Response: RefreshToken cho mã refresh token với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeToken(RevokeRefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu RevokeToken gọi với: {RefreshToken}", request.RefreshToken);
            var result = await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
            _logger.LogInformation("API Response: RevokeToken cho mã refresh token với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        [HttpPost("UserLogOut")]
        [Authorize]
        public async Task<IActionResult> UserLogOut(LogOutRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu LogOut");
            var result = await _authService.LogoutAsync(request, cancellationToken);
            _logger.LogInformation("API Response: LogOut với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        // Gửi mã OTP đến email người dùng
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordEmailRequestDTO forgot, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu ForgotPassword gọi với: {Email}", forgot.Email);
            var result = await _authService.GenerateAndSendOtpEmailAsync(forgot, cancellationToken);
            _logger.LogInformation("API Response: ForgotPassword cho email {Email} với trạng thái {Status}", forgot.Email, result.Status);
            return Ok(result);
        }

        // Xác thực mã OTP cho việc quên mật khẩu
        [HttpPost("VerifyForgotPassword")]
        public async Task<IActionResult> VerifyForgotPassword(VerifyOtpEmailRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu VerifyForgotPasswordOtp gọi với: {Email}", request.Email);
            var result = await _authService.VerifyForgotPasswordOtpAsync(request, cancellationToken);
            _logger.LogInformation("API Response: VerifyForgotPasswordOtp cho email {Email} với trạng thái {Status}", request.Email, result.Status);
            return Ok(result);
        }

        // Xác thực mã OTP và đặt lại mật khẩu
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(VerifyOtpEmailResetPasswordRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu VerifyOtpForResetPassword gọi với: {Email}", request.Email);
            var result = await _authService.VerifyOtpForResetPasswordAsync(request, cancellationToken);
            _logger.LogInformation("API Response: VerifyOtpForResetPassword cho email {Email} với trạng thái {Status}", request.Email, result.Status);
            return Ok(result);
        }

        // Gửi mã OTP đến email mới khi đổi email
        [HttpPost("ChangeEmail")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail(ChangeEmailOtpRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu GenerateAndSendOtpChangeEmail");
            var result = await _authService.GenerateAndSendOtpChangeEmailAsync(request, cancellationToken);
            _logger.LogInformation("API Response: GenerateAndSendOtpChangeEmail với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        // Xác thực mã OTP và đổi email
        [HttpPost("VerifyChangeEmail")]
        [Authorize]
        public async Task<IActionResult> VerifyChangeEmail(VerifyChangeEmailRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Response: Bắt đầu VerifyOtpChangeEmail");
            var result = await _authService.VerifyOtpForChangeEmailAsync(request, cancellationToken);
            _logger.LogInformation("API Response: VerifyOtpChangeEmail với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            var result = await _authService.GetAllAsync(page, limit, cancellationToken);
            return Ok(result);
        }
    }
}
