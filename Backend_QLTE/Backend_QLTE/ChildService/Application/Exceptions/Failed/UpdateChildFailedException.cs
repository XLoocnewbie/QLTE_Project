using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class UpdateChildFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public UpdateChildFailedException(Guid childId)
            : base($"Cập nhật thông tin trẻ Id = {childId} thất bại.") { }
    }
}
