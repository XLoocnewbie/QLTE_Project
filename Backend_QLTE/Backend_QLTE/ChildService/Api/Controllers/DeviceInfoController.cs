using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceInfoController : ControllerBase
    {
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly ILogger<DeviceInfoController> _logger;

        public DeviceInfoController(
            ILogger<DeviceInfoController> logger,
            IDeviceInfoService deviceInfoService)
        {
            _logger = logger;
            _deviceInfoService = deviceInfoService;
        }

        // 🟢 Lấy tất cả thiết bị
        [HttpGet("GetAll")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetAll(int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API: GetAll DeviceInfo page={Page}, limit={Limit}", page, limit);
            var result = await _deviceInfoService.GetAllAsync(page, limit, cancellationToken);
            return Ok(result);
        }

        // 🔵 Lấy chi tiết thiết bị
        [HttpGet("Detail/{deviceId}")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetDetail(Guid deviceId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API: GetDetail DeviceInfo Id={Id}", deviceId);
            var result = await _deviceInfoService.GetDetailAsync(deviceId, cancellationToken);
            return Ok(result);
        }

        // 🟡 Lấy thiết bị theo ChildId
        [HttpGet("GetByChild")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetByChild(Guid childId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API: GetByChild DeviceInfo ChildId={ChildId}", childId);
            var result = await _deviceInfoService.GetByChildAsync(childId, cancellationToken);
            return Ok(result);
        }

        // 🟢 Tạo mới thiết bị
        [HttpPost("Create")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Create([FromBody] DeviceInfoCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: Create DeviceInfo ChildId={ChildId}, UserId={UserId}", dto.ChildId, userId);

            var result = await _deviceInfoService.CreateAsync(dto, userId, cancellationToken);
            return Ok(result);
        }

        // 🟠 Cập nhật thiết bị
        [HttpPut("Update")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Update([FromBody] DeviceInfoUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: Update DeviceInfo Id={DeviceId}, UserId={UserId}", dto.DeviceId, userId);

            var result = await _deviceInfoService.UpdateAsync(dto, userId, cancellationToken);
            return Ok(result);
        }

        // 🔴 Xoá thiết bị
        [HttpDelete("Delete")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> Delete([FromQuery] DeviceInfoDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: Delete DeviceInfo Id={DeviceId}, UserId={UserId}", dto.DeviceId, userId);

            var result = await _deviceInfoService.DeleteAsync(dto, userId, cancellationToken);
            return Ok(result);
        }

        // ⚡ Cập nhật pin / trạng thái (Realtime)
        [HttpPut("UpdateStatus")]
        [AllowAnonymous] // Cho phép thiết bị tự cập nhật
        public async Task<IActionResult> UpdateStatus(Guid deviceId, int? pin, bool? online, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API: UpdateStatus DeviceInfo Id={DeviceId}", deviceId);
            var result = await _deviceInfoService.UpdateStatusAsync(deviceId, pin, online, cancellationToken);
            return Ok(result);
        }

        // 🔒 Khoá thiết bị (Parent gửi lệnh)
        [HttpPut("LockDevice/{childId}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> LockDevice(Guid childId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: LockDevice ChildId={ChildId}, UserId={UserId}", childId, userId);

            var result = await _deviceInfoService.LockDeviceAsync(childId, userId, cancellationToken);
            return Ok(result);
        }

        // 🔓 Mở khoá thiết bị
        [HttpPut("UnlockDevice/{childId}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> UnlockDevice(Guid childId, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: UnlockDevice ChildId={ChildId}, UserId={UserId}", childId, userId);

            var result = await _deviceInfoService.UnlockDeviceAsync(childId, userId, cancellationToken);
            return Ok(result);
        }

        // 🆕 Bật/Tắt theo dõi định kỳ
        [HttpPut("SetTracking/{childId}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> SetTracking(Guid childId, [FromQuery] bool isTracking, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";
            _logger.LogInformation("API: SetTracking ChildId={ChildId}, UserId={UserId}, isTracking={isTracking}", childId, userId, isTracking);

            var result = await _deviceInfoService.SetTrackingStateAsync(childId, isTracking, userId, cancellationToken);
            return Ok(result);
        }
    }
}
