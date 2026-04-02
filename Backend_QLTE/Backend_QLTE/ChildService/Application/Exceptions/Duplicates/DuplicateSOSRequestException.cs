using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateSOSRequestException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateSOSRequestException(Guid childId, DateTime thoiGian)
            : base($"Đã tồn tại yêu cầu SOS 'Đang xử lý' cho ChildId = {childId} (thời gian gửi {thoiGian:dd/MM/yyyy HH:mm:ss}).") { }
    }
}
