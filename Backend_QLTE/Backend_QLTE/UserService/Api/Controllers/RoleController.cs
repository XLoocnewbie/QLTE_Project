using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_QLTE.UserService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        // Tạo Role mới
        [HttpPost("Create")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(RoleCreateDTO createRole, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Bắt đầu CreateRole với role {RoleName}", createRole.RoleName);
            var result = await _roleService.CreateRoleAsync(createRole);
            _logger.LogInformation("API Response: Tạo Role {RoleName} với trạng thái {Status}", createRole.RoleName, result.Status);
            return Ok(result);
        }

        // Lấy tất cả Role với phân trang
        [HttpGet("GetListRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListUser([FromQuery]GetListRoleRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Bắt đầu GetListRole với Page {Page} và Limit {Limit}", request.Page, request.Limit);
            var result = await _roleService.GetAllRoleAsync(request, cancellationToken);
            _logger.LogInformation("API Response: Lấy danh sách Role với trạng thái {Status}", result.Status);
            return Ok(result);
        }

        [HttpGet("GetRoleByRoleName")]
        public async Task<IActionResult> GetRoleByRoleName([FromQuery] FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("API Request: Bắt đầu GetRoleByRoleName với RoleName {RoleName}", request.RoleName);
            var result = await _roleService.GetRoleByRoleNameAsync(request, cancellationToken);
            _logger.LogInformation("API Response: Tìm Role theo tên với trạng thái {Status}", result.Status);
            return Ok(result);
        }
    }
}
