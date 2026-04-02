using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.Interfaces.Factories;
using Backend_QLTE.UserService.Application.Interfaces.Orchestrators;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Domain.Services.Interfaces;

namespace Backend_QLTE.UserService.Application.Orchestrators
{
    public class RoleOrchestrator : IRoleOrchestrator
    {
        private readonly IRoleFactory _roleFactory;
        private readonly IValidationService _validationService;
        private readonly IRoleDomainService _roleDomainService;
        private readonly ILogger<RoleOrchestrator> _logger;

        public RoleOrchestrator(IRoleFactory roleFactory, IValidationService validationService 
            , IRoleDomainService roleDomainService, ILogger<RoleOrchestrator> logger)
        {
            _roleFactory = roleFactory;
            _validationService = validationService;
            _roleDomainService = roleDomainService;
            _logger = logger;
        }

        //Tạo Role mới
        public async Task<Role> CreateRoleAsync(RoleCreateDTO createRole, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu CreateRoleAsync với role {RoleName}", createRole.RoleName);
            // Check Role đã tồn tại chưa
            await _validationService.ValidateAsync(createRole,cancellationToken);

            // Tạo đối tượng Role mới
            var role = _roleFactory.CreateRole(createRole);

            _roleDomainService.EnsureCanCreate(role); // Kiểm tra các điều kiện để tạo Role mới

            _logger.LogInformation("[Orchestrator] Role '{RoleName}' đã được validate và khởi tạo thành công", createRole.RoleName);
            return role;
        }

        // Lấy danh sách Role với phân trang
        public async Task<(List<Role> role, int total, int last)> GetAllRoleAsync(GetListRoleRequestDTO request,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu GetAllRoleAsync");
            _roleDomainService.EnsureCanGetListRole(request.Page, request.Limit); // Kiểm tra các điều kiện để lấy danh sách Role

            var roles = await _validationService.ValidateAsync<GetListRoleRequestDTO, (List<Role> role, int total, int last)>(request, cancellationToken);

            _logger.LogInformation("Orchestrator: Hoàn thành GetAllRoleAsync với {Count} roles",roles.total);
            return roles;
        }

        // Tìm Role theo tên
        public async Task<(List<Role> role, int total, int last)> GetRoleByRoleNameAsync(FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu FindRoleByRoleName với RoleName {RoleName}", request.RoleName);

            _roleDomainService.EnsureCanFindUserName(request.RoleName, request.Page, request.Limit);
            // Kiểm tra các điều kiện để tìm Role theo tên
            var roles = await _validationService.ValidateAsync<FindRoleByRoleNameRequestDTO, (List<Role> role, int total, int last)>(request, cancellationToken);
            
            return roles;
        }
    }
}
