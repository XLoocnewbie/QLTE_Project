using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Failed
{
    public class GenerateTokenUserFailedException : DomainException
    {
        public GenerateTokenUserFailedException()
            : base($"Tạo token thất bại!") { }
    }
}
