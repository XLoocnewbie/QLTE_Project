using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Failed
{
    public class ResetPasswordFailedException : BusinessException
    {
        public override int StatusCode { get; }
        public ResetPasswordFailedException(string msg, int statusCode = 500)
            : base(msg) 
        {
            StatusCode = statusCode;
        }
    }
}
