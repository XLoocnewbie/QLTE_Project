using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudyPeriodController : ControllerBase
    {
        private readonly IStudyPeriodService _studyPeriodService;
        private readonly ILogger<StudyPeriodController> _logger;

        public StudyPeriodController(
            ILogger<StudyPeriodController> logger,
            IStudyPeriodService studyPeriodService)
        {
            _logger = logger;
            _studyPeriodService = studyPeriodService;
        }

        // 🟢 Lấy tất cả StudyPeriod theo trẻ (GET)
        [HttpGet("GetAllByChild")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetAllByChild(Guid childId, int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetAll StudyPeriod cho ChildId={ChildId}", childId);
            var result = await _studyPeriodService.GetAllStudyPeriodsByChildAsync(childId, page, limit, cancellationToken);
            _logger.LogInformation("API Response: GetAll StudyPeriod ChildId={ChildId} Status={Status}", childId, result.Status);
            return Ok(result);
        }

        // 🟢 Lấy chi tiết 1 khung giờ học
        [HttpGet("Detail/{studyPeriodId}")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetDetail(Guid studyPeriodId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetDetail StudyPeriod Id={Id}", studyPeriodId);
            var result = await _studyPeriodService.GetDetailAsync(studyPeriodId, cancellationToken);
            _logger.LogInformation("API Response: GetDetail StudyPeriod Id={Id} Status={Status}", studyPeriodId, result.Status);
            return Ok(result);
        }

        // 🟢 Tạo mới StudyPeriod (khung giờ học)
        [HttpPost("Create")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Create([FromBody] StudyPeriodCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Create StudyPeriod cho ChildId={ChildId}", dto.ChildId);
            var result = await _studyPeriodService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Create StudyPeriod ChildId={ChildId} Status={Status}", dto.ChildId, result.Status);
            return Ok(result);
        }

        // 🟡 Cập nhật StudyPeriod
        [HttpPut("Update")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Update([FromBody] StudyPeriodUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Update StudyPeriod Id={Id}", dto.StudyPeriodId);
            var result = await _studyPeriodService.UpdateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Update StudyPeriod Id={Id} Status={Status}", dto.StudyPeriodId, result.Status);
            return Ok(result);
        }

        // 🔴 Xóa StudyPeriod
        [HttpDelete("Delete")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Delete([FromQuery] StudyPeriodDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Delete StudyPeriod Id={Id}", dto.StudyPeriodId);
            var result = await _studyPeriodService.DeleteAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Delete StudyPeriod Id={Id} Status={Status}", dto.StudyPeriodId, result.Status);
            return Ok(result);
        }

        // 🟣 Bật / tắt khung giờ học (toggle IsActive)
        [HttpPatch("ToggleActive")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> ToggleActive(Guid studyPeriodId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: ToggleActive StudyPeriod Id={Id}", studyPeriodId);
            var result = await _studyPeriodService.ToggleActiveAsync(studyPeriodId, cancellationToken);
            _logger.LogInformation("API Response: ToggleActive StudyPeriod Id={Id} Status={Status}", studyPeriodId, result.Status);
            return Ok(result);
        }

        // 🟣 API: Lấy khung giờ học đang hoạt động của 1 đứa trẻ
        [HttpGet("GetActiveByChild")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> GetActiveByChild(Guid childId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("API Request: GetActiveByChild ChildId={ChildId}", childId);

            var result = await _studyPeriodService.GetActiveByChildAsync(childId, cancellationToken);

            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }

    }
}
