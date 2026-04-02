using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Failed
{
    public class GenerateRefreshTokenFailedException : DomainException
    {
        public GenerateRefreshTokenFailedException() : base("Tạo mới refresh token thất bại")
        {
        }
    }
}
