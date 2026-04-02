using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Invalids
{
    public class InvalidSOSRequestException : BusinessException
    {
        public override int StatusCode => 400;

        public InvalidSOSRequestException(string reason)
            : base($"Dữ liệu yêu cầu SOS không hợp lệ: {reason}.") { }
    }
}
