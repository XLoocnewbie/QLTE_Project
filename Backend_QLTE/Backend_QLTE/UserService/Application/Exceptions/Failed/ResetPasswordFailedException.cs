using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Failed
{
    public class ResetPasswordFailedException : BusinessException
    {
        public override int StatusCode => 500;
        public ResetPasswordFailedException(string? account = "")
            : base($"Đặt lại mật khẩu cho {account} không thành công, vui lòng thử lại sau.") { }
    }
}
