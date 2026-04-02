using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.NotFounds
{
    public class UserEmailNotFoundException : BusinessException
    {
        public override int StatusCode => 404;
        public UserEmailNotFoundException(string? email)
            : base($"Không tìm thấy người dùng với email: {email ?? "unknown"}")
        {
        }
    }
}
