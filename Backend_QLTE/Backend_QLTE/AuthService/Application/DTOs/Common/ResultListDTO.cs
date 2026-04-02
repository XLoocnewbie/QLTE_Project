namespace Backend_QLTE.AuthService.Application.DTOs.Common
{
    public class ResultListDTO<T>
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }
        public PaginationDTO? pagination { get; set; }

        // Thành công
        public static ResultListDTO<T> Success(T data, string msg = "", PaginationDTO pagination = null)
            => new ResultListDTO<T> { Status = true, Msg = msg, Data = data , pagination = pagination };

        public static ResultListDTO<T> Success(T data, string msg = "")
            => new ResultListDTO<T> { Status = true, Msg = msg, Data = data };

        // Thất bại
        public static ResultListDTO<T> Fail(string msg)
            => new ResultListDTO<T> { Status = false, Msg = msg };
    }
}
