using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend_QLTE.Hubs;                
using Microsoft.AspNetCore.SignalR;

namespace Backend_QLTE.ChildService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SOSRequestController : ControllerBase
    {
        private readonly ISOSRequestService _sosRequestService;
        private readonly ILogger<SOSRequestController> _logger;
        private readonly IHubContext<SOSHub> _hubContext;

        public SOSRequestController(
            ILogger<SOSRequestController> logger,
            ISOSRequestService sosRequestService,
            IHubContext<SOSHub> hubContext)
        {
            _logger = logger;
            _sosRequestService = sosRequestService;
            _hubContext = hubContext;
        }

        // 🟢 Lấy tất cả SOSRequest (GET)
        [HttpGet("GetAll")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetAll SOSRequest page={Page}, limit={Limit}", page, limit);
            var result = await _sosRequestService.GetAllAsync(page, limit, cancellationToken);
            _logger.LogInformation("API Response: GetAll SOSRequest Status={Status}", result.Status);
            return Ok(result);
        }

        // 🟢 Lấy danh sách SOSRequest theo trẻ (GET)
        [HttpGet("GetByChild")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetByChild(Guid childId, int page = 1, int limit = 10, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetByChild SOSRequest ChildId={ChildId}", childId);
            var result = await _sosRequestService.GetByChildAsync(childId, page, limit, cancellationToken);
            _logger.LogInformation("API Response: GetByChild SOSRequest ChildId={ChildId} Status={Status}", childId, result.Status);
            return Ok(result);
        }

        // 🔵 Lấy chi tiết 1 yêu cầu SOS
        [HttpGet("Detail/{sosId}")]
        [Authorize(Roles = "Parent,Admin")]
        public async Task<IActionResult> GetDetail(Guid sosId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: GetDetail SOSRequest Id={Id}", sosId);
            var result = await _sosRequestService.GetDetailAsync(sosId, cancellationToken);
            _logger.LogInformation("API Response: GetDetail SOSRequest Id={Id} Status={Status}", sosId, result.Status);
            return Ok(result);
        }

        // 🟢 Tạo mới SOSRequest
        [HttpPost("Create")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Create([FromBody] SOSRequestCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Create SOSRequest ChildId={ChildId}", dto.ChildId);
            var result = await _sosRequestService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Create SOSRequest ChildId={ChildId} Status={Status}", dto.ChildId, result.Status);

            // ✅ Nếu tạo thành công → broadcast tới nhóm childId
            if (result.Status && result.Data != null)
            {
                var sos = result.Data; // thường là SOSResponseDTO hoặc SOSRequest entity

                await _hubContext.Clients.Group(dto.ChildId.ToString()).SendAsync("ReceiveSOS", new
                {
                    sosId = sos.SOSId,
                    childId = sos.ChildId,
                    thoiGian = sos.ThoiGian,
                    viDo = sos.ViDo,
                    kinhDo = sos.KinhDo,
                    trangThai = sos.TrangThai
                });

                _logger.LogInformation("🚨 SOS realtime sent to group {GroupId}", dto.ChildId);
            }

            return Ok(result);
        }

        // 🟠 Cập nhật SOSRequest
        [HttpPut("Update")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Update([FromBody] SOSRequestUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Update SOSRequest Id={Id}", dto.SOSId);
            var result = await _sosRequestService.UpdateAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Update SOSRequest Id={Id} Status={Status}", dto.SOSId, result.Status);
            return Ok(result);
        }

        // 🔴 Xóa SOSRequest
        [HttpDelete("Delete")]
        [Authorize(Roles = "Parent,Admin,Children")]
        public async Task<IActionResult> Delete([FromQuery] SOSRequestDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Delete SOSRequest Id={Id}", dto.SOSId);
            var result = await _sosRequestService.DeleteAsync(dto, cancellationToken);
            _logger.LogInformation("API Response: Delete SOSRequest Id={Id} Status={Status}", dto.SOSId, result.Status);
            return Ok(result);
        }
    }
}
