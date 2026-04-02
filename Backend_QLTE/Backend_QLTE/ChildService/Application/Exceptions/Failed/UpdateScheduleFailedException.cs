using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateScheduleFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateScheduleFailedException(Guid scheduleId)
            : base($"Cập nhật Schedule Id = {scheduleId} thất bại.") { }
    }
}
