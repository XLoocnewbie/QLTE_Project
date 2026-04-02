using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService _examScheduleService;
        private readonly ILogger<ExamScheduleController> _logger;

        public ExamScheduleController(
            ILogger<ExamScheduleController> logger,
            IExamScheduleService examScheduleService)
        {
            _logger = logger;
            _examScheduleService = examScheduleService;
        }

        // 🟢 Lấy tất cả lịch thi theo trẻ (GET)
        [HttpGet("GetAllByChild")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> GetAllByChild(Guid childId, int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetAll ExamSchedule cho ChildId={ChildId}", childId);
            var result = await _examScheduleService.GetAllByChildAsync(childId, page, limit, cancellationToken);
            _logger.LogInformation("API Response: GetAll ExamSchedule ChildId={ChildId} Status={Status}", childId, result.Status);
            return Ok(result);
        }

        // 🟢 Lấy chi tiết 1 lịch thi
        [HttpGet("Detail/{examId}")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> GetDetail(Guid examId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetDetail ExamSchedule Id={Id}", examId);
            var result = await _examScheduleService.GetDetailAsync(examId, cancellationToken);
            _logger.LogInformation("API Response: GetDetail ExamSchedule Id={Id} Status={Status}", examId, result.Status);
            return Ok(result);
        }

        // 🕓 Lấy danh sách lịch thi sắp tới
        [HttpGet("Upcoming")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> GetUpcoming(Guid childId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetUpcoming ExamSchedule cho ChildId={ChildId}", childId);
            var result = await _examScheduleService.GetUpcomingExamsAsync(childId, cancellationToken);
            _logger.LogInformation("API Response: GetUpcoming ExamSchedule ChildId={ChildId} Status={Status}", childId, result.Status);
            return Ok(result);
        }

        // 🟢 Tạo mới ExamSchedule
        [HttpPost("Create")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> Create([FromBody] ExamScheduleCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Create ExamSchedule cho ChildId={ChildId}", dto.ChildId);
            var result = await _examScheduleService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Create ExamSchedule ChildId={ChildId} Status={Status}", dto.ChildId, result.Status);
            return Ok(result);
        }

        // 🟡 Cập nhật ExamSchedule
        [HttpPut("Update")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> Update([FromBody] ExamScheduleUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Update ExamSchedule Id={Id}", dto.ExamId);
            var result = await _examScheduleService.UpdateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Update ExamSchedule Id={Id} Status={Status}", dto.ExamId, result.Status);
            return Ok(result);
        }

        // 🔴 Xóa ExamSchedule
        [HttpDelete("Delete")]
        [Authorize(Roles = "User,Admin,Children")]
        public async Task<IActionResult> Delete([FromQuery] ExamScheduleDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Delete ExamSchedule Id={Id}", dto.ExamId);
            var result = await _examScheduleService.DeleteAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Delete ExamSchedule Id={Id} Status={Status}", dto.ExamId, result.Status);
            return Ok(result);
        }
    }
}
