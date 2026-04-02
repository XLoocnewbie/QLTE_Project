using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("GetLocationHistoryNew")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetLocationHistoryNew(Guid childId)
        {
            var result = await _locationService.GetLocationHistoryNewAsync(childId);
            return Ok(result);
        }
    }
}
