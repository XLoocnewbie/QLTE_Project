using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.NotFounds
{
    public class RoleNotFoundException : BusinessException
    {
        public override int StatusCode => 404;
        public RoleNotFoundException() : base("Role không tồn tại trong hệ thống!")
        {
        }
    }
}
