using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class SOSRequestFailedException : DomainException
    {
        public SOSRequestFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với yêu cầu SOS thất bại: {reason}.") { }
    }
}
