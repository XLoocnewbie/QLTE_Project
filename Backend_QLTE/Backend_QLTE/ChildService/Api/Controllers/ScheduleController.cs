using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(
            ILogger<ScheduleController> logger,
            IScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }

        // 🟢 Lấy tất cả Schedule theo trẻ (GET)
        [HttpGet("GetAllByChild")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetAllByChild(Guid childId, int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetAll Schedule cho ChildId={ChildId}", childId);
            var result = await _scheduleService.GetAllByChildAsync(childId, page, limit, cancellationToken);
            _logger.LogInformation("API Response: GetAll Schedule ChildId={ChildId} Status={Status}", childId, result.Status);
            return Ok(result);
        }

        // 🟢 Lấy chi tiết 1 Schedule
        [HttpGet("Detail/{scheduleId}")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetDetail(Guid scheduleId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetDetail Schedule Id={Id}", scheduleId);
            var result = await _scheduleService.GetDetailAsync(scheduleId, cancellationToken);
            _logger.LogInformation("API Response: GetDetail Schedule Id={Id} Status={Status}", scheduleId, result.Status);
            return Ok(result);
        }

        // 🟢 Tạo mới Schedule (Lịch học)
        [HttpPost("Create")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Create([FromBody] ScheduleCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Create Schedule cho ChildId={ChildId}", dto.ChildId);
            var result = await _scheduleService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Create Schedule ChildId={ChildId} Status={Status}", dto.ChildId, result.Status);
            return Ok(result);
        }

        // 🟡 Cập nhật Schedule
        [HttpPut("Update")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Update([FromBody] ScheduleUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Update Schedule Id={Id}", dto.ScheduleId);
            var result = await _scheduleService.UpdateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Update Schedule Id={Id} Status={Status}", dto.ScheduleId, result.Status);
            return Ok(result);
        }

        // 🔴 Xóa Schedule
        [HttpDelete("Delete")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Delete([FromQuery] ScheduleDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Delete Schedule Id={Id}", dto.ScheduleId);
            var result = await _scheduleService.DeleteAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Delete Schedule Id={Id} Status={Status}", dto.ScheduleId, result.Status);
            return Ok(result);
        }
    }
}
