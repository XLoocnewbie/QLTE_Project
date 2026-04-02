using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Invalid
{
    public class InvalidRefreshTokenException : BusinessException
    {
        public override int StatusCode => base.StatusCode;

        public InvalidRefreshTokenException() : base("Refresh token không hợp lệ hoặc để trống")
        {
        }
    }
}
