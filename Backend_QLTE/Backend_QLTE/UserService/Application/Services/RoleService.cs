using Backend_QLTE.UserService.Application.DTOs.Admin.Role;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.DTOs.Common;
using Backend_QLTE.UserService.Application.Exceptions.Failed;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Application.Interfaces.Orchestrators;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Services;

namespace Backend_QLTE.UserService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleOrchestrator _roleOrchestrator;
        private readonly IRoleResponseMapper _roleResponseMapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;
        public RoleService(IUnitOfWork unitOfWork, IRoleOrchestrator roleOrchestrator
            , IRoleResponseMapper roleResponseMapper, IPaginationMapper paginationMapper
            , ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleOrchestrator = roleOrchestrator;
            _roleResponseMapper = roleResponseMapper;
            _paginationMapper = paginationMapper;
            _logger = logger;
        }

        //Role

        // Get list Role
        public async Task<ResultListDTO<RoleResponseDTO>> GetAllRoleAsync(GetListRoleRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: Bắt đầu GetAllRoleAsync với page {Page} và limit {Limit}", request.Page, request.Limit);
            var (roles, total, last) = await _roleOrchestrator.GetAllRoleAsync(request, cancellationToken);

            var roleDtos = roles.Select(u => _roleResponseMapper.toDto(u)).ToList();
            var pagination = _paginationMapper.ToDto(request.Page, request.Limit, total, last);

            _logger.LogInformation("ApplicationService: Lấy danh sách Role thành công với tổng số {Total} và trang cuối {Last}", total, last);
            return ResultListDTO<RoleResponseDTO>.Success(roleDtos,"Lấy danh sách role thành công.",pagination);
        }

        //Tạo Role mới
        public async Task<ResultDTO> CreateRoleAsync(RoleCreateDTO createRole, CancellationToken cancellationToken = default)
        {
            // Tạo Role mới
            _logger.LogInformation("ApplicationService: Bắt đầu CreateRoleAsync với role {RoleName}", createRole.RoleName);

            var role = await _roleOrchestrator.CreateRoleAsync(createRole, cancellationToken);

            // Bắt đầu transaction
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Tạo Role mới
            var result = await _unitOfWork.Roles.CreateRoleAsync(role);
            if (!result.Succeeded)
            {
                _logger.LogError("ApplicationService: Tạo Role {RoleName} thất bại!", role.RoleName);
                await transaction.RollbackAsync(cancellationToken);
                throw new CreateRoleFailedException(role.RoleName);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationService: Tạo Role {RoleName} thành công!", role.RoleName);
            return ResultDTO.Success($"Tạo role {role.RoleName} mới thành công");
        }

        // Tìm Role theo tên
        public async Task<ResultListDTO<RoleResponseDTO>> GetRoleByRoleNameAsync(FindRoleByRoleNameRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: Bắt đầu FindRoleByRoleName với RoleName {RoleName}, page {Page} và limit {Limit}", request.RoleName, request.Page, request.Limit);
            var (roles, total, last) = await _roleOrchestrator.GetRoleByRoleNameAsync(request, cancellationToken);

            var roleDtos = roles.Select(u => _roleResponseMapper.toDto(u)).ToList();

            var pagination = _paginationMapper.ToDto(request.Page, request.Limit, total, last);

            _logger.LogInformation("ApplicationService: Tìm Role theo tên {RoleName} thành công với tổng số {Total} và trang cuối {Last}", request.RoleName, total, last);
            return ResultListDTO<RoleResponseDTO>.Success(roleDtos, "Tìm kiếm role thành công.", pagination);
        }
    }
}
