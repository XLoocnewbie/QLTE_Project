using Backend_QLTE.UserService.shared.Exceptions;
using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class CreateUserFailedException : BusinessException
    {
        public override int StatusCode => 500;
        public CreateUserFailedException(string email) 
            : base($"Cập nhật thông tin người dùng {email} thất bại") { }
    }
}
