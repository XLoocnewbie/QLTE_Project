namespace Backend_QLTE.UserService.Shared.Kernel
{
    public class Result<T>
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public T Value { get; set; }

        public static Result<T> Success(T value, string msg = "")
            => new Result<T> { Status = true, Msg = msg, Value = value };

        public static Result<T> Fail(string msg)
            => new Result<T> { Status = false, Msg = msg };
    }
    public class Result
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public string? Value { get; set; }

        public static Result Success(string value, string msg = "")
            => new Result { Status = true, Msg = msg, Value = value };

        public static Result Fail(string msg)
            => new Result { Status = false, Msg = msg };
    }
}
