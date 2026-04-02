using Backend_QLTE.AuthService.Domain.Exceptions.Failed;
using Backend_QLTE.AuthService.Domain.Exceptions.Invalid;
using Backend_QLTE.AuthService.shared.Exceptions;
using Backend_QLTE.AuthService.Shared.Exceptions;
using System.Net;

namespace Backend_QLTE.AuthService.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Dictionary mapping exception type → HTTP status
        private static readonly Dictionary<Type, int> ExceptionStatusMap = new()
        {
            //failed
            { typeof(GenerateTokenUserFailedException), StatusCodes.Status400BadRequest },
            { typeof(GenerateRefreshTokenFailedException), StatusCodes.Status400BadRequest },
            { typeof(CreateOtpFailedException), StatusCodes.Status400BadRequest },
            //invalid
            { typeof(InvalidTokenException), StatusCodes.Status401Unauthorized },
            { typeof(InvalidExpiredRefreshTokenException), StatusCodes.Status401Unauthorized },
            { typeof(InvalidOtpException), StatusCodes.Status401Unauthorized },
            { typeof(InvalidRefreshTokenException), StatusCodes.Status401Unauthorized }

        };

        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ áp dụng middleware cho các endpoint bắt đầu bằng /api/Auth
            if (!context.Request.Path.StartsWithSegments("/api/Auth"))
            {
                await _next(context);
                return;
            }


            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain error occurred");

                int statusCode = ExceptionStatusMap.TryGetValue(ex.GetType(), out var code)
                    ? code
                    : StatusCodes.Status400BadRequest;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = false,
                    Msg = ex.Message,
                });
            }
            catch (BusinessException ex) // lỗi nghiệp vụ
            {
                _logger.LogWarning(ex, "Business error occurred.");

                context.Response.StatusCode = ex.StatusCode;
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = false,
                    Msg = ex.Message
                });
            }
            catch (Exception ex) // lỗi khác
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = false,
                    Msg = $"Đã xảy ra lỗi hệ thống, vui lòng thử lại sau."
                });
            }
        }
    }
}
