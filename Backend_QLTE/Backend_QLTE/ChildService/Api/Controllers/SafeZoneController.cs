using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SafeZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public SafeZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet("GetSafeZoneByUserId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSafeZoneByUserId(string userId)
        {
            var result = await _zoneService.GetSafeZoneByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("GetSafeZoneByUserIdAndChildId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSafeZoneByUserIdAndChildId(string userId, Guid childId)
        {
            var result = await _zoneService.GetSafeZoneByUserIdAndChildIdAsync(userId,childId);
            return Ok(result);
        }

        [HttpPost("CreateSafeZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> CreateSafeZone(CreateSafeZoneRequestDTO request)
        {
            var result = await _zoneService.CreateSafeZoneAsync(request);
            return Ok(result);
        }

        [HttpPut("UpdateSafeZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> UpdateSafeZone(UpdateSafeZoneRequestDTO request)
        {
            var result = await _zoneService.UpdateSafeZoneAsync(request);
            return Ok(result);
        }

        [HttpDelete("DeleteSafeZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> DeleteSafeZone(Guid safeZoneId)
        {
            var result = await _zoneService.DeleteSafeZoneAsync(safeZoneId);
            return Ok(result);
        }
    }
}
