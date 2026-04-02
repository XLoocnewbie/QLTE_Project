using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class DeleteUserFailedException : BusinessException
    {
        public override int StatusCode => base.StatusCode;

        public DeleteUserFailedException(string? userName) : base($"Xóa người dùng '{userName}' thất bại!")
        {
        }
    }
}
