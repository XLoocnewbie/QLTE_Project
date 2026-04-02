using Backend_QLTE.UserService.shared.Exceptions;

namespace Backend_QLTE.UserService.Domain.Exceptions.Invalid
{
    public class InvalidTenNDException : DomainException
    {
        public InvalidTenNDException(string? tenND)
            : base($"Tên người dùng không hợp lệ: '{tenND ?? string.Empty}'. Tên người dùng không được để trống và không được chứa ký tự đặc biệt.")
        {
        }
    }
}
