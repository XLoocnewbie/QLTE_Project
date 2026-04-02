using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Domain.Exceptions.NotFounds
{
    public class StudyPeriodNotFoundException : DomainException
    {
        public StudyPeriodNotFoundException(string studyPeriodId)
            : base($"Không tìm thấy khung giờ học với Id = {studyPeriodId}.") { }
    }
}
