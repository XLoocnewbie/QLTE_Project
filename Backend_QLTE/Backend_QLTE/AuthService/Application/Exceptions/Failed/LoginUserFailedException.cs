using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Failed
{
    public class LoginUserFailedException : BusinessException
    {
        public override int StatusCode { get; } // Unauthorized
        public LoginUserFailedException(string message , int statusCode = 500)
            : base($"Đăng nhập thất bại: {message}")
        {
            StatusCode = statusCode;
        }
    }
}
