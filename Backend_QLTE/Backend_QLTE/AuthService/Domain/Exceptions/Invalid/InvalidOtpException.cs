using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Invalid
{
    public class InvalidOtpException : DomainException
    {
        public InvalidOtpException(string code) : base($"Mã OTP {code} không hợp lệ hoặc đã hết hạn!")
        {
        }
    }
}
