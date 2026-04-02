using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class GenerateResetTokenFailedException : BusinessException
    {
        public override int StatusCode => 500;
        public GenerateResetTokenFailedException(string? account)
            : base($"Tạo mã token để đặt lại mật khẩu cho {account} không thành công.") { }
    }
}
