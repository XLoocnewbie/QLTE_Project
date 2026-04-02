using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class UserNotFoundByEmailException : BusinessException
    {
        public override int StatusCode => 404;
        public UserNotFoundByEmailException(string email) 
            : base($"Không tìm thấy người dùng với email: {email}") { }
    }
}
