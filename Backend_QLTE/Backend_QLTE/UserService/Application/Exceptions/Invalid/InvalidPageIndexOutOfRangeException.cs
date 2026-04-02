using Backend_QLTE.UserService.Shared.Exceptions;

namespace Backend_QLTE.UserService.Application.Exceptions.Invalid
{
    public class InvalidPageIndexOutOfRangeException : BusinessException
    {
        public override int StatusCode => 400;
        public InvalidPageIndexOutOfRangeException(int pageIndex, int totalPages)
            : base($"Chỉ số trang {pageIndex} không hợp lệ. Chỉ số trang phải nằm trong khoảng từ 1 đến {totalPages}.") { }
    }
}
