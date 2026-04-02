using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class StudyPeriodFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public StudyPeriodFailedException(string action, string reason)
            : base($"Không thể {action} StudyPeriod: {reason}") { }
    }
}
