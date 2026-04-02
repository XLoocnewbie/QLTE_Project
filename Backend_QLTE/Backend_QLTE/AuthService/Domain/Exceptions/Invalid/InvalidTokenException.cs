using Backend_QLTE.AuthService.shared.Exceptions;

namespace Backend_QLTE.AuthService.Domain.Exceptions.Invalid
{
    public class InvalidTokenException : DomainException
    {
        public InvalidTokenException() 
            : base("Token không hợp lệ hoặc đã hết hạn.") {}
    }
}
