using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Invalid
{
    public class InvalidExpiredRefreshTokenException : DomainException
    {
        public InvalidExpiredRefreshTokenException() : base("Refresh token đã hết hạn hoặc bị khóa")
        {
        }
    }
}
