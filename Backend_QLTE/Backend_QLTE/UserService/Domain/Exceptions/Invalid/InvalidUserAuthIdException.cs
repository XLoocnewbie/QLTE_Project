using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserAuthIdException : DomainException
    {
        public InvalidUserAuthIdException(string? userAuthId)
            : base($"AuthId người dùng không được để trống: {userAuthId}.")
        {
        }
    }
}
