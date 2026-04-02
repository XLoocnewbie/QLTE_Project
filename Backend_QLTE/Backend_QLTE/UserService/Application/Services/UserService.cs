using Backend_QLTE.UserService.Application.DTOs.Common;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Interfaces.Services;
using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Orchestrators;
using Backend_QLTE.UserService.Infrastructure.Config;
using Microsoft.Extensions.Options;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Application.Exceptions.Failed;
using Backend_QLTE.UserService.Application.Exceptions.Invalid;
using Shared.FileStorage;
using Backend_QLTE.UserService.Application.Exceptions.NotFounds;
using Backend_QLTE.UserService.Application.Exceptions.Duplicates;
using Backend_QLTE.UserService.Domain.Services.Interfaces;
using System.Security.Claims;
using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Domain.Entities;
using System.Net.WebSockets;

namespace Backend_QLTE.UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDomainService _userDomainService;
        private readonly IUserOrchestrator _userOrchestrator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserResponseMapper _userResponseMapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly string _defaultRole; // Vai trò mặc định khi tạo tài khoản
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;


        public UserService(IUnitOfWork unitOfWork , IUserDomainService userDomainService
            , IUserOrchestrator userOrchestrator, IOptions<UserSettings> userSettings
            , IUserResponseMapper userResponseMapper  , IPaginationMapper paginationMapper
            ,IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            //tham số của phương thức bị null nhưng không được phép null.
            _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
            _userOrchestrator = userOrchestrator ?? throw new ArgumentNullException(nameof(userOrchestrator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userResponseMapper = userResponseMapper ?? throw new ArgumentNullException(nameof(userResponseMapper));
            _paginationMapper = paginationMapper ?? throw new ArgumentNullException(nameof(paginationMapper));
            _defaultRole = userSettings.Value.DefaultRole ?? "User";
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger;
        }

        // USER
        // Đăng ký tài khoản Local
        public async Task<ResultDTO> RegisterUserAsync(UserRegisterDTO registerUser, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đăng ký user {Email}", registerUser.Email);

            var user = await _userOrchestrator.RegisterUserAsync(registerUser, cancellationToken);

            // await using → gọi DisposeAsync() (async).
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Tạo User mới
            var result = await _unitOfWork.Users.RegisterUserAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning("ApplicationSerVice: Đăng ký user {Email} thất bại!", registerUser.Email);
                throw new CreateUserFailedException(registerUser.Email);
            }

            // Gán role cho User mới tạo
            var resultAssign = await _unitOfWork.Users.AssignRoleToUserAsync(user, _defaultRole);
            if (!resultAssign.Succeeded)
            {
                _logger.LogError("ApplicationSerVice: Gán vai trò cho user {Email} thất bại!", user.Email);
                await transaction.RollbackAsync(cancellationToken);
                throw new AssignRoleFailedException(user.Email, _defaultRole);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Đăng ký user {Email} thành công!", user.Email);

            return ResultDTO.Success($"Đăng ký tài khoản {user.Email} thành công");
        }

        // Login tài khoản từ bên ngoài nếu email chưa có đăng ký
        public async Task<ResultDTO<ExternalLoginUserResponseDTO>> ProviderLoginOrRegisterUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đăng nhập hoặc đăng ký user {Email} bên thứ 3", request.Email);

            // Kiểm tra user đã tồn tại chưa
            var existingUser = await _unitOfWork.Users.FindByAuthIdAndTypeLoginAsync(request.AuthId, request.TypeLogin, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogInformation("ApplicationSerVice: User {Email} chưa tồn tại, bắt đầu đăng ký mới", request.Email);

                // Chưa có user thì đăng ký mới
                await ProviderRegisterUserAsync(request, cancellationToken);

                return await ProviderLoginUserAsync(request, cancellationToken);
            }
            else
            {
                // Đã có user thì login
                _logger.LogInformation("ApplicationSerVice: User {Email} đã tồn tại, bắt đầu đăng nhập", request.Email);
                return await ProviderLoginUserAsync(request, cancellationToken);
            }

        }


        // Login tài khoản từ bên ngoài nếu email chưa có đăng ký
        public async Task<ResultDTO> ProviderRegisterUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đăng ký user {Email} bên thứ 3", request.Email);

            var user = await _userOrchestrator.ProviderRegisterUserAsync(request, cancellationToken);

            // await using → gọi DisposeAsync() (async).
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Đăng ký User mới
            var result = await _unitOfWork.Users.RegisterUserNotPasswordAsync(user);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogWarning("ApplicationSerVice: Đăng ký user {Email} bên thứ 3 thất bại!", request.Email);
                throw new CreateUserFailedException(request.Email);
            }

            // Gán role cho User mới tạo
            var resultAssign = await _unitOfWork.Users.AssignRoleToUserAsync(user, _defaultRole);
            if (!resultAssign.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Gán vai trò cho user {Email} bên thứ 3 thất bại!", request.Email);
                throw new AssignRoleFailedException(user.Email, _defaultRole);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Đăng ký user {Email} bên thứ 3 thành công!", request.Email);
            return ResultDTO.Success($"Đăng ký tài khoản {user.Email} thành công");
        }

        // Login tài khoản từ bên ngoài nếu email chưa có đăng ký
        public async Task<ResultDTO<ExternalLoginUserResponseDTO>> ProviderLoginUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đăng nhập user {Email} bên thứ 3", request.Email);

            // Đăng nhập User từ bên ngoài
            var userLogin = _userOrchestrator.ProviderLoginUser(request);

            // Tìm user trong database
            var user = await _unitOfWork.Users.FindByAuthIdAndTypeLoginAsync(userLogin.AuthId, userLogin.TypeLogin, cancellationToken);

            // Kiểm tra user đã tồn tại chưa
            var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
            if (roles == null || !roles.Any())
            {
                _logger.LogError("ApplicationSerVice: User {Email} không có vai trò!", user.Email);
                throw new InvalidUserHasNoRoleException(user.Email);
            }
            // Trả về kết quả login
            user.Roles = roles.ToList();
            _logger.LogInformation("ApplicationSerVice: Đăng nhập user {Email} bên thứ 3 thành công!", user.Email);
            return ResultDTO<ExternalLoginUserResponseDTO>.Success(_userResponseMapper.ToExternalLogin(user), $"Đăng Nhập bằng '{user.TypeLogin}' thành công");
        }

        // Update Info người dùng
        public async Task<ResultDTO<UpdateInfoUserResponseDTO>> UpdateInfoUserAsync(InfoUserUpdateRequestDTO update, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(": Bắt đầu cập nhật thông tin user {UserName}", update.UserName);
            string userId;
            if (string.IsNullOrWhiteSpace(update.UserId))
            {
                // Lấy userId từ token
                userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value; // User mặc định có Claims là rỗng chứ không null.
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Orchestrator: Không xác định được UserId đã gửi update!");
                    throw new UserNotFoundByIdException(userId);
                }
            }
            else
            {
                userId = update.UserId;
            }

            var userUpdate = await _userOrchestrator.UpdateInfoUserAsync(update, userId, cancellationToken);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Tìm người dùng theo userId
            var user = await _unitOfWork.Users.FindByUserIdAsync(userUpdate.Id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("ApplicationSerVice: Người dùng không tồn tại với Id {UserId}", userUpdate.Id);
                throw new UserNotFoundException(userUpdate.UserName);
            }
            user.UserName = update.UserName;
            user.PhoneNumber = update.PhoneNumber;

            // Check xem người dùng có thay đổi avatar không
            if (update.AvatarND != null && update.AvatarND.Length > 0)
            {
                // Thư mục lưu trữ ảnh
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");

                // Nếu User có avatar cũ thì xóa
                if (!string.IsNullOrEmpty(user.AvatarND))
                {
                    FileHelper.DeleteImage(user.AvatarND);
                }

                // Lưu ảnh mới
                var fileName = await FileHelper.SaveImageAsync(update.AvatarND, folderPath);
                user.AvatarND = Path.Combine("uploads", "avatars", fileName).Replace("\\", "/"); // Lưu đường dẫn tương đối

            }

            user.NameND = update.NameND;
            user.UserName = update.UserName;
            user.GioiTinh = update.GioiTinh;
            user.MoTa = update.MoTa;

            // Cập nhật thông tin người dùng
            var resultUpdate = await _unitOfWork.Users.UpdateUserAsync(user);
            if (resultUpdate == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Cập nhật thông tin người dùng {UserName} thất bại!", user.UserName);
                throw new UpdateUserFailedException(update.UserName);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Cập nhật thông tin người dùng {UserName} thành công!", user.UserName);
            return ResultDTO<UpdateInfoUserResponseDTO>.Success(_userResponseMapper.ToUpdate(user), $"Cập nhật thông tin người dùng '{user.Email}' thành công.");


        }


        // Login và sinh ra token
        public async Task<ResultDTO<LoginLocalUserResponseDTO>> LoginLocalUserAsync(LoginRequestDTO requestLogin, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đăng nhập user {Email}", requestLogin.Account);

            var user = await _userOrchestrator.LoginLocalUserAsync(requestLogin, cancellationToken);

            // Kiểm tra user đã tồn tại chưa
            var CheckPass = await _unitOfWork.Users.CheckPasswordAsync(user, requestLogin.Password);
            if (!CheckPass)
            {
                _logger.LogWarning("ApplicationSerVice: Đăng nhập user {Account} thất bại do sai mật khẩu!", requestLogin.Account);
                throw new InvalidPasswordException();
            }

            // Kiểm tra user có Role không
            var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
            if (roles == null || !roles.Any())
            {
                _logger.LogError("ApplicationSerVice: User {Account} không có vai trò!", requestLogin.Account);
                throw new InvalidUserHasNoRoleException(requestLogin.Account);
            }
            // Trả về kết quả login
            user.Roles = roles.ToList();
            _logger.LogInformation("ApplicationSerVice: Đăng nhập user {Account} thành công!", requestLogin.Account);
            return ResultDTO<LoginLocalUserResponseDTO>.Success(_userResponseMapper.ToLocalLogin(user), "Đăng nhập thành công");
        }

        // Tìm User theo email
        public async Task<ResultDTO<UserResponseDTO>> FindUserByEmailAsync(FindUserByEmailRequestDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Tìm kiếm người dùng với Email {Email}", dto.Email);

            var userFind = _userOrchestrator.GetUserByEmail(dto);

            // Tìm người dùng theo email
            var user = await _unitOfWork.Users.FindByEmailAsync(userFind.Email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("ApplicationSerVice: Người dùng không tồn tại với Email {Email}", userFind.Email);
                throw new UserNotFoundException(userFind.Email);
            }

            // Kiểm tra user có Role không
            var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
            if (roles == null || !roles.Any())
            {
                _logger.LogError("ApplicationSerVice: User {Account} không có vai trò!", user.Email);
                throw new InvalidUserHasNoRoleException(user.Email);
            }
            // Trả về kết quả login
            user.Roles = roles.ToList();
            _logger.LogInformation("ApplicationSerVice: Tìm thấy người dùng với Email {Email}", userFind.Email);
            return ResultDTO<UserResponseDTO>.Success(_userResponseMapper.ToDto(user), $"Tìm thấy người dùng '{userFind.Email}' thành công.");
        }

        // Tìm User theo userId
        public async Task<ResultDTO<UserResponseDTO>> FindUserByUserIdAsync(FindUserByIdRequestDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Tìm kiếm người dùng với id {id}", dto.UserId);

            var userFind = _userOrchestrator.GetUserByUserId(dto);

            // Tìm người dùng theo email
            var user = await _unitOfWork.Users.FindByUserIdAsync(userFind.Id, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("ApplicationSerVice: Người dùng không tồn tại với id {id}", userFind.Id);
                throw new UserNotFoundException(userFind.Email);
            }
            // Kiểm tra user có Role không
            var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
            if (roles == null || !roles.Any())
            {
                _logger.LogError("ApplicationSerVice: User {Account} không có vai trò!", user.Email);
                throw new InvalidUserHasNoRoleException(user.Email);
            }
            // Trả về kết quả login
            user.Roles = roles.ToList();
            _logger.LogInformation("ApplicationSerVice: Tìm thấy người dùng với Email {Email}", userFind.Email);
            return ResultDTO<UserResponseDTO>.Success(_userResponseMapper.ToDto(user), $"Tìm thấy người dùng '{userFind.Email}' thành công.");
        }

        // Tìm User theo NameND
        public async Task<ResultListDTO<UserResponseDTO>> FindUserByTenNDAsync(FindUserByTenNDRequestTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Tìm kiếm người dùng với Tên người dùng {TenND}", dto.TenND);
            // Tìm người dùng theo email
            var users = await _userOrchestrator.GetUserByNameND(dto, cancellationToken);
            foreach (var user in users.user)
            {
                // Lấy vai trò của người dùng
                var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
                if (roles == null || !roles.Any())
                {
                    _logger.LogError("ApplicationSerVice: User {Account} không có vai trò!", user.Email);
                    throw new InvalidUserHasNoRoleException(user.Email);
                }
                user.Roles = roles.ToList();
            }
            // Trả về kết quả login
            var userDtos = users.user.Select(u => _userResponseMapper.ToDto(u)).ToList();
            var pagination = _paginationMapper.ToDto(dto.Page ,dto.Limit, users.total, users.last);

            _logger.LogInformation("ApplicationSerVice: Tìm thấy người dùng với Tên {TenND}", dto.TenND);
            return ResultListDTO<UserResponseDTO>.Success(userDtos, $"Tìm thấy người dùng '{dto.TenND}' thành công.",pagination);
        }

        // Reset mật khẩu
        public async Task<ResultDTO> ResetPasswordAsync(ResetPasswordRequestDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu đặt lại mật khẩu cho userId {UserId}", dto.UserId);
            // Lấy thông tin từ dto
            var userReset = _userOrchestrator.ResetPasswordAsync(dto);

            // Đặt lại mật khẩu
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Tìm người dùng theo userId
            var userResponse = await _unitOfWork.Users.FindByUserIdAsync(dto.UserId, cancellationToken);
            if (userResponse == null)
            {
                _logger.LogWarning("ApplicationSerVice: Người dùng không tồn tại");
                throw new UserNotFoundException();
            }

            // Check mật khẩu mới không được trùng với mật khẩu cũ
            var checkPass = await _unitOfWork.Users.CheckPasswordAsync(userResponse, dto.NewPassword);
            if (checkPass)
            {
                _logger.LogWarning("ApplicationSerVice: Mật khẩu mới không được trùng với mật khẩu cũ của người dùng {UserName}", userResponse.UserName);
                throw new DuplicateResetPasswordException(userResponse.Email);
            }

            // Tạo token đặt lại mật khẩu
            var tokenReset = await _unitOfWork.Users.GeneratePasswordResetTokenAsync(userResponse);
            if (string.IsNullOrEmpty(tokenReset))
            {
                _logger.LogError("ApplicationSerVice: Tạo token đặt lại mật khẩu cho người dùng {UserName} thất bại!", userResponse.UserName);
                throw new GenerateResetTokenFailedException(userResponse.Email);
            }

            // Đặt lại mật khẩu với token
            var resultReset = await _unitOfWork.Users.ResetPasswordWithTokenAsync(userResponse, tokenReset, dto.NewPassword);
            if (!resultReset.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Đặt lại mật khẩu cho người dùng {UserName} thất bại!", userResponse.UserName);
                throw new ResetPasswordFailedException(userResponse.Email);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Tìm thấy người dùng với UserName {UserName}", userResponse.Email);
            return ResultDTO.Success($"Reset mật khẩu cho Email '{userResponse.Email}' thành công.");
        }

        // Đổi email
        public async Task<ResultDTO> ChangeEmailAsync(ChangeEmailRequestDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu thay đổi email cho userId {UserId}", dto.UserId);

            // Đặt lại mật khẩu
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            // Tìm người dùng theo userId
            var userResponse = await _unitOfWork.Users.FindByUserIdAsync(dto.UserId, cancellationToken);
            if (userResponse == null)
            {
                _logger.LogWarning("ApplicationSerVice: Người dùng không tồn tại");
                throw new UserNotFoundException();
            }

            // Check email mới không được trùng với email của người dùng khác
            var checkEmail = await _unitOfWork.Users.FindByEmailAsync(dto.NewEmail, cancellationToken);
            if (checkEmail != null)
            {
                _logger.LogWarning("ApplicationSerVice: Email mới đã được sử dụng bởi người dùng khác {UserName}", checkEmail.UserName);
                throw new DuplicateEmailException(dto.NewEmail);
            }

            // Lấy thông tin từ dto
            var userChange = _userOrchestrator.ChangeEmail(userResponse, dto.NewEmail);

            // Tạo token thay đổi email
            var tokenChange = await _unitOfWork.Users.GenerateChangeEmailTokenAsync(userChange, dto.NewEmail);
            if (string.IsNullOrEmpty(tokenChange))
            {
                _logger.LogError("ApplicationSerVice: Tạo token thay đổi email cho người dùng {UserName} thất bại!", userChange.UserName);
                throw new GenerateChangeEmailTokenFailedException(userChange.Email, dto.NewEmail);
            }

            // Thay đổi email với token
            var resultChange = await _unitOfWork.Users.ChangeEmailWithTokenAsync(userChange, dto.NewEmail, tokenChange);
            if (!resultChange.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Thay đổi email cho người dùng {UserName} thất bại!", userResponse.Email);
                throw new ChangeEmailFailedException(userResponse.Email, dto.NewEmail);
            }

            var updateTimeChangeEmail = await _unitOfWork.Users.UpdateUserAsync(userChange);
            if (updateTimeChangeEmail == null)
            {
               await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Cập nhật thời gian thay đổi email cho người dùng {UserName} thất bại!", userResponse.Email);
                throw new UpdateUserFailedException(userResponse.UserName);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Thay đổi email cho người dùng {UserName} thành công!", userResponse.Email);
            return ResultDTO.Success($"Thay đổi email cho tài khoản '{userResponse.UserName}' thành công.");
        }

        // QUAN TRI VIEN
        // Lấy danh sách tất cả người dùng
        public async Task<ResultListDTO<UserResponseDTO>> GetAllUsersAsync(ListUserRequestDTO request ,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu lấy danh sách tất cả người dùng");
            
            await _userOrchestrator.GetListUserAsync(request, cancellationToken);

            // Lấy tất cả người dùng
            var users = await _unitOfWork.Users.GetAllUsersAsync(request.page, request.limit,cancellationToken);

            foreach (var user in users.user)
            {
                // Lấy vai trò của người dùng
                var roles = await _unitOfWork.Users.GetRolesUserAsync(user);
                if (roles == null || !roles.Any())
                {
                    _logger.LogError("ApplicationSerVice: User {Account} không có vai trò!", user.Email);
                    throw new InvalidUserHasNoRoleException(user.Email);
                }
                user.Roles = roles.ToList();
            }

            // Map sang DTO trả về
            var userDtos = users.user.Select(user => _userResponseMapper.ToDto(user)).ToList();
            // Phân trang
            var pagination = _paginationMapper.ToDto(request.page, request.limit, users.total, users.last);

            _logger.LogInformation("ApplicationSerVice: Lấy danh sách tất cả người dùng thành công với {Count} người dùng", users.total);
            return ResultListDTO<UserResponseDTO>.Success(userDtos, "Lấy danh sách tất cả người dùng thành công",pagination);
        }

        // Xóa người dùng
        public async Task<ResultDTO> DeleteUserAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationSerVice: Bắt đầu xóa người dùng với Id {UserId}", request.UserId);
            var user = await _userOrchestrator.DeleteUserAsync(request);
            // Xóa người dùng
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var delete = await _unitOfWork.Users.DeleteUserAsync(user);
            if (!delete.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationSerVice: Xóa người dùng {UserName} thất bại!", user.UserName);
                throw new DeleteUserFailedException(user.UserName);
            }
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationSerVice: Xóa người dùng {UserName} thành công!", user.UserName);
            return ResultDTO.Success($"Xóa người dùng '{user.UserName}' thành công.");
        }
    }
}
        
