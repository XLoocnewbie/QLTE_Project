using Backend_QLTE.UserService.shared.Exceptions;
using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class CreateRoleFailedException : BusinessException
    {
        public override int StatusCode => base.StatusCode;
        public CreateRoleFailedException(string roleName) 
            : base($"Tạo role '{roleName}' mới thất bại.")
        {
        }
    }
}
