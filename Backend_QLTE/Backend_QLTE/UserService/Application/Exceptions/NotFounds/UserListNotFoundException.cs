using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class UserListNotFoundException : BusinessException
    {
        public override int StatusCode => 404;
        public UserListNotFoundException()
            : base("Không tìm thấy danh sách người dùng nào.") { }
    }
}
