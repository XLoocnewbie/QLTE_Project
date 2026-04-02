using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateStudyPeriodFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateStudyPeriodFailedException(Guid studyPeriodId)
            : base($"Cập nhật StudyPeriod Id = {studyPeriodId} thất bại.") { }
    }
}
