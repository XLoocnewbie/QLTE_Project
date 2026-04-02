using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Failed
{
    public class RevokeRefreshTokenFailedException : BusinessException
    {
        public override int StatusCode => base.StatusCode; 

        public RevokeRefreshTokenFailedException() : base ("Thu hồi Refresh Token thất bại!")
        {
        }
    }
}
