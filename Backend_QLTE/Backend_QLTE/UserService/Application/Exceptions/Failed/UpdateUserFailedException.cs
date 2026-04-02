using Backend_QLTE.UserService.shared.Exceptions;
using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class UpdateUserFailedException : BusinessException
    {
        public override int StatusCode => 500;
        public UpdateUserFailedException(string userName)
        : base($"Cập nhật thông tin người dùng {userName} thất bại")
        {
        }
    }
}
