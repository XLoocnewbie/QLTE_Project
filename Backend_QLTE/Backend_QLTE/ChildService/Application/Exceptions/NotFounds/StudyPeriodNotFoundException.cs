using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.NotFounds
{
    public class StudyPeriodNotFoundException : BusinessException
    {
        public override int StatusCode => 404;

        public StudyPeriodNotFoundException()
            : base("Không tìm thấy bất kỳ StudyPeriod nào.") { }

        public StudyPeriodNotFoundException(Guid studyPeriodId)
            : base($"Không tìm thấy StudyPeriod với Id = {studyPeriodId}") { }
    }
}
