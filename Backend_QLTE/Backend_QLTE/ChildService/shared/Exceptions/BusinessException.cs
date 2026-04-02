namespace Backend_QLTE.ChildService.Shared.Exceptions
{
    public abstract class BusinessException : Exception
    {
        public virtual int StatusCode => 400; // Mã trạng thái HTTP mặc định là 400 (Bad Request)
        public BusinessException(string message) : base(message) { }
    }
}
