using Backend_QLTE.AuthService.Shared.Exceptions;

namespace Backend_QLTE.AuthService.Application.Exceptions.Invalid
{
    public class InvalidAuthProviderException : BusinessException
    {
        public override int StatusCode => 400; // 400 Bad Request
        public InvalidAuthProviderException(string provider)
            : base($"Nhà cung cấp xác thực '{provider}' không hợp lệ hoặc không được hỗ trợ.") { }
    }
}
