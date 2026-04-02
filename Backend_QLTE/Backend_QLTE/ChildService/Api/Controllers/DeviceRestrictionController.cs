using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceRestrictionController : ControllerBase
    {
        private readonly IDeviceRestrictionService _restrictionService;

        public DeviceRestrictionController(IDeviceRestrictionService restrictionService)
        {
            _restrictionService = restrictionService;
        }

        // 🟢 Lấy danh sách cấu hình hạn chế theo DeviceId
        [HttpGet("GetAllByDevice")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetAllByDevice(Guid deviceId)
        {
            var result = await _restrictionService.GetByDeviceIdAsync(deviceId);
            return Ok(result);
        }

        // 🔵 Lấy chi tiết cấu hình
        [HttpGet("GetDetail")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetDetail(Guid restrictionId)
        {
            var result = await _restrictionService.GetDetailAsync(restrictionId);
            return Ok(result);
        }

        // 🟡 Tạo mới cấu hình hạn chế
        [HttpPost("Create")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Create([FromBody] DeviceRestrictionCreateDTO dto)
        {
            var result = await _restrictionService.CreateAsync(dto);
            return Ok(result);
        }

        // 🟠 Cập nhật cấu hình hạn chế
        [HttpPut("Update")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Update([FromBody] DeviceRestrictionUpdateDTO dto)
        {
            var result = await _restrictionService.UpdateAsync(dto);
            return Ok(result);
        }

        // 🔴 Xoá cấu hình hạn chế
        [HttpDelete("Delete")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Delete([FromBody] DeviceRestrictionDeleteDTO dto)
        {
            var result = await _restrictionService.DeleteAsync(dto);
            return Ok(result);
        }

        // 🟣 Bật / tắt firewall
        [HttpPatch("ToggleFirewall")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> ToggleFirewall(Guid restrictionId)
        {
            var result = await _restrictionService.ToggleFirewallAsync(restrictionId);
            return Ok(result);
        }

        // ✅ Bật Restriction StudyMode
        [HttpPost("ActivateStudyRestriction")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> ActivateStudyRestriction(Guid deviceId)
        {
            var result = await _restrictionService.ActivateStudyRestrictionAsync(deviceId);
            return Ok(result);
        }

        // 🚫 Tắt Restriction
        [HttpPost("DeactivateRestriction")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> DeactivateRestriction(Guid deviceId)
        {
            var result = await _restrictionService.DeactivateRestrictionAsync(deviceId);
            return Ok(result);
        }
    }
}
