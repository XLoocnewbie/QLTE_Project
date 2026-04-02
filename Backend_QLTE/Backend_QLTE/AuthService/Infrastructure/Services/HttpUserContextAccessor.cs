using Backend_QLTE.AuthService.Application.Interfaces.Services;
using System.Security.Claims;

namespace Backend_QLTE.AuthService.Infrastructure.Services
{
    public class HttpUserContextAccessor : IHttpUserContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpUserContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
