using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class UserNotFoundByIdException : BusinessException 
    {
        public override int StatusCode => 404;
        public UserNotFoundByIdException(string? id)
            : base($"Không tìm thấy người dùng với id: {id ?? string.Empty}") { }
    }
}
