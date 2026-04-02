using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Invalid
{
    public class InvalidRefreshTokenException : DomainException
    {
        public InvalidRefreshTokenException() : base("Refresh token không hợp lệ hoặc để trống")
        {
        }
    }
}
