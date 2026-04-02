using Backend_QLTE.AuthService.Domain.Models;

namespace Backend_QLTE.AuthService.Domain.Interfaces.Services
{
    public interface IOtpDomainService
    {
        bool VerifyOtp(Otp otp, string code); // Xác thực mã OTP
        Otp CreateOtp(string userId, string type); // Tạo mã OTP mới
    }
}
