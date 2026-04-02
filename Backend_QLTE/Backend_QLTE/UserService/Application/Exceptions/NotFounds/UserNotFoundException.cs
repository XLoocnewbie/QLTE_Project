using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class UserNotFoundException : BusinessException
    {
        public override int StatusCode => 404; 
        public UserNotFoundException(string? userName = "")
            : base($"Không tìm thấy người dùng với tài khoản: {userName}") { }
    }
}
