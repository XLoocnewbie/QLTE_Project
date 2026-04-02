using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.NotFounds
{
    public class UserNotFoundByIdException : BusinessException
    {
        public override int StatusCode => 404;

        public UserNotFoundByIdException(string? userId)
            : base($"Không tìm thấy người dùng với ID: {userId ?? "nuknown"}")
        {
        }
    }
}
