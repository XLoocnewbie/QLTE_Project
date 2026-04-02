using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidUserPageLimitException : DomainException
    {
        public InvalidUserPageLimitException(int limit)
            : base($"Giá trị giới hạn trang không hợp lệ: {limit}. Giá trị phải nằm trong khoảng từ 1 đến 100.")
        {
        }
    }
}
