using System.Net;

namespace Backend_QLTE.ChildService.shared.Exceptions
{

    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = (int)HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
