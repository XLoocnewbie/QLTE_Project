using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Failed
{
    public class CreateOtpFailedException : DomainException
    {
        public CreateOtpFailedException() : base($"Tạo otp thất bại!")
        {
        }
    }
}
