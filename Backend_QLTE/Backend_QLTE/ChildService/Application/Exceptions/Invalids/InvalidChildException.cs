using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidChildException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidChildException(string reason)
            : base($"Dữ liệu trẻ không hợp lệ: {reason}") { }
    }
}
