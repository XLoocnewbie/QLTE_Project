using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Invalid
{
    public class InvalidTokenException : BusinessException
    {
        public override int StatusCode => 401; // Unauthorized

        public InvalidTokenException()
            : base("Token không hợp lệ hoặc đã hết hạn.")
        {
        }
    }
}
