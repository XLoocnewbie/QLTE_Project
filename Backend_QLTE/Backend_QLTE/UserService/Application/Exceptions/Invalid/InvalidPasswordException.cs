using Backend_QLTE.UserService.shared.Exceptions;
using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Invalid
{
    public class InvalidPasswordException : BusinessException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public InvalidPasswordException()
            : base("Mật khẩu không hợp lệ không chính xác") { }
    }
}
