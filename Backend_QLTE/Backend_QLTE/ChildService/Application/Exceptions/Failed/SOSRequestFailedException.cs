using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class SOSRequestFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public SOSRequestFailedException(string action, string reason)
            : base($"Không thể {action} yêu cầu SOS: {reason}.") { }
    }
}
