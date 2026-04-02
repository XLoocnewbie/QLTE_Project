using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Failed
{
    public class CreateChildFailedException : BusinessException
    {
        public override int StatusCode => 500;

        public CreateChildFailedException(string userId)
            : base($"Tạo mới thông tin trẻ cho phụ huynh (UserId={userId}) thất bại.") { }
    }
}
