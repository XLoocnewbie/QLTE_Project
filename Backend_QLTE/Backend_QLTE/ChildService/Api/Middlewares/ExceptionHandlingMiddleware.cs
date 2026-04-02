using Backend_QLTE.ChildService.Domain.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Domain.Exceptions.Failed;
using Backend_QLTE.ChildService.Domain.Exceptions.Invalid;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Domain.Exceptions;
using Backend_QLTE.ChildService.Shared.Exceptions;
using System.Net;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.shared.Exceptions;
//using Backend_QLTE.ChildService.Domain.Exceptions.Unauthorized;

namespace Backend_QLTE.ChildService.Api.Middlewares
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

        private static readonly Dictionary<Type, int> ExceptionStatusMap = new()
        {
            // StudyPrediost
            { typeof(DuplicateStudyPeriodException), StatusCodes.Status400BadRequest },
            { typeof(StudyPeriodFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidStudyPeriodException), StatusCodes.Status400BadRequest },
            { typeof(StudyPeriodNotFoundException), StatusCodes.Status404NotFound },

            // SOSRequest
            { typeof(DuplicateSOSRequestException), StatusCodes.Status400BadRequest },
            { typeof(SOSRequestFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidSOSRequestException), StatusCodes.Status400BadRequest },
            { typeof(SOSRequestNotFoundException), StatusCodes.Status404NotFound },

            // DeviceInfo
            { typeof(DuplicateDeviceInfoException), StatusCodes.Status400BadRequest },
            { typeof(DeviceInfoFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidDeviceInfoException), StatusCodes.Status400BadRequest },
            { typeof(DeviceInfoNotFoundException), StatusCodes.Status404NotFound },

            // Child
            { typeof(DuplicateChildException), StatusCodes.Status400BadRequest },
            { typeof(ChildFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidChildException), StatusCodes.Status400BadRequest },
            { typeof(ChildNotFoundException), StatusCodes.Status404NotFound },

            // Schedule
            { typeof(DuplicateScheduleException), StatusCodes.Status400BadRequest },
            { typeof(ScheduleFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidScheduleException), StatusCodes.Status400BadRequest },
            { typeof(ScheduleNotFoundException), StatusCodes.Status404NotFound },

            // ExamSchedule
            { typeof(DuplicateExamScheduleException), StatusCodes.Status400BadRequest },
            { typeof(ExamScheduleFailedException), StatusCodes.Status500InternalServerError },
            { typeof(InvalidExamScheduleException), StatusCodes.Status400BadRequest },
            { typeof(ExamScheduleNotFoundException), StatusCodes.Status404NotFound },

        };

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/Child") &&
                !context.Request.Path.StartsWithSegments("/api/Message") &&
                !context.Request.Path.StartsWithSegments("/api/DangerZone") &&
                !context.Request.Path.StartsWithSegments("/api/SafeZone") &&
                !context.Request.Path.StartsWithSegments("/api/StudyPeriod") &&
                !context.Request.Path.StartsWithSegments("/api/SOSRequest") &&
                !context.Request.Path.StartsWithSegments("/api/DeviceInfo") &&
                !context.Request.Path.StartsWithSegments("/api/Schedule") &&
                !context.Request.Path.StartsWithSegments("/api/ExamSchedule") 
                ) 
            {
                await _next(context);
                return;
            }
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";

                var result = new
                {
                    Status = false,
                    Msg = ex.Message
                };

                await context.Response.WriteAsJsonAsync(result);
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
