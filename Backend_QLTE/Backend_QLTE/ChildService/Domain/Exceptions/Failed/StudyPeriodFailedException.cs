using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Failed
{
    public class StudyPeriodFailedException : DomainException
    {
        public StudyPeriodFailedException(string action, string reason)
            : base($"Thực hiện '{action}' với khung giờ học thất bại: {reason}.") { }
    }
}
