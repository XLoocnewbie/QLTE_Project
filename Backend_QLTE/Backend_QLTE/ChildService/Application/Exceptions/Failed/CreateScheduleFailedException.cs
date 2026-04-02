using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateScheduleFailedException(string childId)
            : base($"Tạo Schedule cho ChildId = {childId} thất bại.") { }
    }
}
