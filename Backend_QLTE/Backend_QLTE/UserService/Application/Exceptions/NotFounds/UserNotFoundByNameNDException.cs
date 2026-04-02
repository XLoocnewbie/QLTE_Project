using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class UserNotFoundByNameNDException : BusinessException
    {
        public override int StatusCode => 404;
        public UserNotFoundByNameNDException(string? nameND = "")
            : base($"Không tìm thấy người dùng với tên người dùng: {nameND ?? string.Empty}") { }
    }
}
