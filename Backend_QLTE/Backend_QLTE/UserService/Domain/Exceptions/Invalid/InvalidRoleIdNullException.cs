using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidRoleIdNullException : DomainException
    {
        public InvalidRoleIdNullException() : base("Role Id không được để trống!")
        {
        }
    }
}
