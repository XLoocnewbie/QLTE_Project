using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidSOSRequestException : DomainException
    {
        public InvalidSOSRequestException(string reason)
            : base($"Yêu cầu SOS không hợp lệ: {reason}.") { }
    }
}
