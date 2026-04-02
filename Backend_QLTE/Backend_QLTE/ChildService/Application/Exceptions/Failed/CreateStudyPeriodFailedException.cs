using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateStudyPeriodFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateStudyPeriodFailedException(string childId)
            : base($"Tạo StudyPeriod cho ChildId = {childId} thất bại.") { }
    }
}
