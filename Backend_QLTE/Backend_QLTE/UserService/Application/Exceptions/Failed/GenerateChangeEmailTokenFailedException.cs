using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class GenerateChangeEmailTokenFailedException : BusinessException
    {
        public override int StatusCode => base.StatusCode; 

        public GenerateChangeEmailTokenFailedException(string email, string newEmail)
            : base($"Tạo mã token để đổi email $'{email}' thành '{newEmail}' thất bại.")
        {
        }
    }
}
