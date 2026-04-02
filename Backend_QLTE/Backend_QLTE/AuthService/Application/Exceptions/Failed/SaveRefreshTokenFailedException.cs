using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Failed
{
    public class SaveRefreshTokenFailedException : BusinessException
    {
        public override int StatusCode => base.StatusCode;
        public SaveRefreshTokenFailedException() : base("Lưu refresh token thất bại")
        {
        }
    }
}
