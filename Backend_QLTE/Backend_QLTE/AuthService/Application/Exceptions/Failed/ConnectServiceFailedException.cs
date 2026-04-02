using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Failed
{
    public class ConnectServiceFailedException : BusinessException
    {
        public override int StatusCode => 503; // Service Unavailable 

        public ConnectServiceFailedException(string serviceName)
            : base($"Kết nối đến dịch vụ {serviceName} thất bại. Vui lòng thử lại sau.")
        {
        }
    }
}
