using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class ChildFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public ChildFailedException(string action, string reason)
            : base($"Không thể {action} thông tin trẻ: {reason}") { }
    }
}
