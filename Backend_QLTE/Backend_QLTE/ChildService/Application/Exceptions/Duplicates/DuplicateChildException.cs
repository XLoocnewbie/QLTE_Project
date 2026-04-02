using Backend_QLTE.ChildService.Shared.Exceptions;

namespace Backend_QLTE.ChildService.Application.Exceptions.Duplicates
{
    public class DuplicateChildException : BusinessException
    {
        public override int StatusCode => 409;

        public DuplicateChildException(string name)
            : base($"Trẻ '{name}' đã tồn tại trong hệ thống.") { }

        public DuplicateChildException(Guid childId)
            : base($"Trẻ với Id = {childId} đã tồn tại trong hệ thống.") { }
    }
}
