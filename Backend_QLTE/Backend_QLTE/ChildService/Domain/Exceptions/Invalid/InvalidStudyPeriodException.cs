using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Invalid
{
    public class InvalidStudyPeriodException : DomainException
    {
        public InvalidStudyPeriodException(string reason)
            : base($"Khung giờ học không hợp lệ: {reason}.") { }
    }
}
