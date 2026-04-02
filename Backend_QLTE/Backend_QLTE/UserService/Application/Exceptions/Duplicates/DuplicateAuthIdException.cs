using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Duplicates
{
    public class DuplicateAuthIdException : BusinessException
    {
        public override int StatusCode => 409;
        public DuplicateAuthIdException(string authId)
            : base($"AuthId '{authId}' của người dùng này đã tồn tại.") { }
    }
}
