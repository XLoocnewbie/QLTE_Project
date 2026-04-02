using Backend_QLTE.UserService.Application.DTOs.Admin.User;
using Backend_QLTE.UserService.Application.DTOs.Client.Login;
using Backend_QLTE.UserService.Application.DTOs.Client.User;
using Backend_QLTE.UserService.Application.Interfaces.Factories;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Application.Interfaces.Orchestrators;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Domain.Services.Interfaces;
namespace Backend_QLTE.UserService.Application.Orchestrators
{
    public class UserOrchestrator : IUserOrchestrator
    {
        private readonly IUserDomainService _userDomainService;
        private readonly IValidationService _validationService;
        private readonly IUserFactory _userFactory;
        private readonly IUserResponseMapper _userResponseMapper;
        private readonly IGuidUserNameGenerator _guidUserNameGenerator;

        private readonly ILogger<UserOrchestrator> _logger;
        public UserOrchestrator(IUserDomainService userDomainService, IValidationService validationService
            , IUserFactory userFactory, IUserResponseMapper userResponseMapper
            , IGuidUserNameGenerator guidUserNameGenerator
            , ILogger<UserOrchestrator> logger)
        {
            _userDomainService = userDomainService;
            _validationService = validationService;
            _userFactory = userFactory;
            _userResponseMapper = userResponseMapper;
            _guidUserNameGenerator = guidUserNameGenerator;
            _logger = logger;
        }

        // Đăng ký tài khoản Local
        public async Task<User> RegisterUserAsync(UserRegisterDTO registerUser, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu RegisterUserAsync với user {Email}", registerUser.Email);
            // Validate dữ liệu đăng ký
            await _validationService.ValidateAsync(registerUser, cancellationToken);

            // username mới đăng ký ngãu nhiên
            var userName = _guidUserNameGenerator.GenerateUserName();

            // Tạo đối tượng User mới
            var user = _userFactory.CreateLocalUser(registerUser, userName);

            // Kiểm tra điều kiện đăng ký
            _userDomainService.EnsureCanRegister(user);

            _logger.LogInformation("Orchestrator: Đăng ký user {Email} thành công!", registerUser.Email);
            return user;
        }

        // Login tài khoản từ bên ngoài nếu email chưa có đăng ký   
        public async Task<User> ProviderRegisterUserAsync(ExternalLoginUserInfoDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu ProviderRegisterUserAsync với user {Email} bên {External}", request.Email, request.TypeLogin);

            await _validationService.ValidateAsync(request, cancellationToken);

            // username mới đăng ký ngãu nhiên
            var userName = _guidUserNameGenerator.GenerateUserName();

            // Tạo đối tượng User mới
            var user = _userFactory.CreateExternalUser(request, userName);

            // Kiểm tra điều kiện đăng ký
            _userDomainService.EnsureCanProviderRegister(user);
            _logger.LogInformation("Orchestrator: Đăng ký user {Email} bên {External} thành công!", request.Email, request.TypeLogin);
            return user;
        }

        // Login tài khoản từ bên ngoài nếu email chưa có đăng ký
        public User ProviderLoginUser(ExternalLoginUserInfoDTO request)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu ProviderLoginUserAsync với user {Email} bên {External}", request.Email, request.TypeLogin);

            // Tạo đối tượng User mới
            var user = _userFactory.CreateExternalUser(request, "");

            // Kiểm tra điều kiện đăng nhập
            _userDomainService.EnsureCanProviderLogin(user);
            _logger.LogInformation("Orchestrator: Đăng nhập user {Email} bên {External} thành công!", user.Email, user.TypeLogin);
            return user;
        }

        // Cập nhật thông tin user
        public async Task<User> UpdateInfoUserAsync(InfoUserUpdateRequestDTO update, string userId,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu UpdateInfoUserAsync với user {UserName}", update.UserName);

            // Validate dữ liệu cập nhật
            await _validationService.ValidateAsync(update, userId, cancellationToken);

            // Tạo đối tượng User từ dữ liệu cập nhật
            var user = _userFactory.CreateUpdateInfoUser(update, userId);

            //Update thông tin user
            _userDomainService.EnsureCanUpdateInfoUser(user);

            _logger.LogInformation("Orchestrator: Cập nhật thông tin user {UserName} thành công!", update.UserName);
            return user;
        }

        // Login local
        public async Task<User> LoginLocalUserAsync(LoginRequestDTO requestLogin , CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu LoginUserAsync với tài khoản {Account}", requestLogin.Account);
            // Validate dữ liệu đăng nhập
            var user = await _validationService.ValidateAsync<LoginRequestDTO,User>(requestLogin,cancellationToken);

            // Đăng nhập user
            _userDomainService.EnsureCanLoginLocal(user);

            _logger.LogInformation("Orchestrator: Đăng nhập user {Account} thành công!", requestLogin.Account);
            
            return user;
        }

        // Tìm user bằng email
        public User GetUserByEmail(FindUserByEmailRequestDTO dto)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu FindUserByEmailAsync với email {Email}", dto.Email);

            // Tạo đối tượng User từ email
            var user = _userFactory.CreateEmail(dto);

            // Validate email
            _userDomainService.EnsureCanUserEmail(user);
            _logger.LogInformation("Orchestrator: Tìm thấy user với email {Email}", dto.Email);
            return user;
        }

        // Tìm user bằng userId
        public User GetUserByUserId(FindUserByIdRequestDTO dto)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu FindUserByIdAsync với userId {UserId}", dto.UserId);
            // Tạo đối tượng User từ userId
            var user = _userFactory.CreateUserId(dto);
            // Validate userId
            _userDomainService.EnsureCanUserId(user);
            _logger.LogInformation("Orchestrator: Tìm thấy user với userId {UserId}", dto.UserId);
            return user;
        }

        // Tìm user bằng tenND
        public async Task<(List<User> user, int total, int last)> GetUserByNameND(FindUserByTenNDRequestTO request,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu FindUserByTenNDAsync với tenND {TenND}", request.TenND);
            // Validate tenND
            _userDomainService.EnsureCanNameND(request.TenND, request.Page, request.Limit);
            var users = await _validationService.ValidateAsync<FindUserByTenNDRequestTO, (List<User> user, int total, int last)>(request,cancellationToken);
            _logger.LogInformation("Orchestrator: Tìm thấy user với tenND {TenND}", request.TenND);
            return (users.user, users.total, users.last);
        }

        // Đổi email
        public User ChangeEmail(User user , string newEmail)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu ChangeEmail với userId {UserId}", user.Id);

            _userDomainService.EnsureCanChangeEmail(user,newEmail);

            _logger.LogInformation("Orchestrator: Thay đổi email thành công cho userId {UserId}", user.Id);
            return user;
        }

        // Đặt lại mật khẩu
        public User ResetPasswordAsync(ResetPasswordRequestDTO dto )
        {
            _logger.LogInformation("Orchestrator: Bắt đầu ResetPasswordAsync với user");
            var user = _userFactory.CreateResetPassword(dto);

            _userDomainService.EnsureCanResetPassword(user);
            _logger.LogInformation("Orchestrator: Reset mật khẩu thành công cho email {Email}", user.Id);
            return user;
        }

        // Lấy danh sách user với phân trang và lọc
        public async Task GetListUserAsync(ListUserRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu GetListUserAsync với PageIndex {PageIndex} và PageSize {PageSize}", request.page, request.limit);
            _userDomainService.EnsureCanGetListUser(request.page, request.limit);
            await _validationService.ValidateAsync(request, cancellationToken);
        }

        public async Task<User> DeleteUserAsync(DeleteUserRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu DeleteUserAsync với userId {UserId}", request.UserId);
            var user = await _validationService.ValidateAsync<DeleteUserRequestDTO,User>(request, cancellationToken);
            _userDomainService.EnsureCanUserId(user);
            await _validationService.ValidateAsync<DeleteUserRequestDTO, User>(request, cancellationToken);
            _logger.LogInformation("Orchestrator: Xóa user thành công với userId {UserId}", request.UserId);

            return user;
        }
        
    }
}
