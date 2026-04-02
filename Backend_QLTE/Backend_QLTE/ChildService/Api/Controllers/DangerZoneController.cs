using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DangerZoneController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public DangerZoneController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet("GetDangerZoneByUserId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDangerZoneByUserId(string userId)
        {
            var result = await _zoneService.GetDangerZoneByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("GetDangerZoneByUserIdAndChildId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDangerZoneByUserIdAndChildId(string userId, Guid childId)
        {
            var result = await _zoneService.GetDangerZoneByUserIdAndChildIdAsync(userId,childId);
            return Ok(result);
        }

        [HttpPost("CreateDangerZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> CreateDangerZone(CreateDangerZoneRequestDTO request)
        {
            var result = await _zoneService.CreateDangerZoneAsync(request);
            return Ok(result);
        }

        [HttpPut("UpdateDangerZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> UpdateDangerZone(UpdateDangerZoneRequestDTO request)
        {
            var result = await _zoneService.UpdateDangerZoneAsync(request);
            return Ok(result);
        }

        [HttpDelete("DeleteDangerZone")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> DeleteDangerZone(Guid dangerZoneId)
        {
            var result = await _zoneService.DeleteDangerZoneAsync(dangerZoneId);
            return Ok(result);
        }
    }
}
