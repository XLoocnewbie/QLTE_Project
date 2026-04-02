using Backend_QLTE.AuthService.Domain.Models;

namespace Backend_QLTE.AuthService.Application.Interfaces.Repositories
{
    public interface IOtpRepository
    {
        Task SaveAsync(Otp otp, CancellationToken cancellationToken = default); // Lưu OTP
        Task<Otp?> GetAsync(string userId, string type, CancellationToken cancellationToken = default); // Lấy OTP theo userId và type
        Task RemoveAsync(string userId, string type, CancellationToken cancellationToken = default); // Xoá OTP theo userId và type
    }
}
