using System.Net;

namespace Backend_QLTE.AuthService.Application.DTOs.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
