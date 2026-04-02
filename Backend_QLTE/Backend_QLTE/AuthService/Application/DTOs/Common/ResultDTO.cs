using System.Net;

namespace Backend_QLTE.AuthService.Application.DTOs.Common
{
    public class ResultDTO<T>
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }

        // Thành công
        public static ResultDTO<T> Success(T data, string msg = "")
            => new ResultDTO<T> { Status = true, Msg = msg ,Data = data};

        // Thất bại
        public static ResultDTO<T> Fail(string msg , int statusCode = 0)
            => new ResultDTO<T> { Status = false, Msg = msg };
    }

    public class ResultDTO
    {
        public bool Status { get; set; }
        public string Msg { get; set; }

        // Thành công
        public static ResultDTO Success(string msg = "")
            => new ResultDTO { Status = true, Msg = msg};

        // Thất bại
        public static ResultDTO Fail(string msg = "")
            => new ResultDTO { Status = false, Msg = msg };
    }
}
