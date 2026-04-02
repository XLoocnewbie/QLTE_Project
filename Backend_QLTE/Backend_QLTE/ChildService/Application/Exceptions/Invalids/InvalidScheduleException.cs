using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidScheduleException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidScheduleException(string reason)
            : base($"Dữ liệu Schedule không hợp lệ: {reason}.") { }
    }
}
