using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.Duplicates
{
    public class DuplicateStudyPeriodException : DomainException
    {
        public DuplicateStudyPeriodException(string description)
            : base($"Khung giờ học '{description}' đã tồn tại.") { }
    }
}
