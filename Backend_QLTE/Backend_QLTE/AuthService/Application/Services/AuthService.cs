using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.Token;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Application.Exceptions.Failed;
using Backend_QLTE.AuthService.Application.Exceptions.Invalid;
using Backend_QLTE.AuthService.Application.Exceptions.NotFounds;
using Backend_QLTE.AuthService.Application.Interfaces.Factories;
using Backend_QLTE.AuthService.Application.Interfaces.Mappers;
using Backend_QLTE.AuthService.Application.Interfaces.Orchestrators;
using Backend_QLTE.AuthService.Application.Interfaces.Repositories;
using Backend_QLTE.AuthService.Application.Interfaces.Services;
using Backend_QLTE.AuthService.Application.Interfaces.Templates;
using Backend_QLTE.AuthService.Domain.Interfaces.Services;
using Backend_QLTE.AuthService.Infrastructure.Data.HttpClients.Interfaces;
using Backend_QLTE.UserService.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.AuthService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthOrchestrator _authOrchestrator;
        private readonly IAuthDomainService _authDomainService;
        private readonly IOtpDomainService _otpDomainService;
        private readonly IUserServiceClient _userServiceClient;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IOtpRepository _otpRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IExternalAuthProvider> _externalAuthProvider;
        private readonly IExternalUserMapper _externalUserMapper;
        private readonly IUserHttpMapper _userHttpMapper;
        private readonly ITokenFactory _tokenFactory;
        private readonly IHttpUserContextAccessor _httpUserContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly UserDbContext _userContext;

        public AuthService(IAuthOrchestrator authOrchestrator , IAuthDomainService authDomainService
            , IUserServiceClient userServiceClient , IOtpDomainService otpDomainService
            , IEmailService emailService , IEmailTemplateService emailTemplateService
            , IOtpRepository otpRepository , IUnitOfWork unitOfWork
            , IEnumerable<IExternalAuthProvider> externalAuthProvider
            , IExternalUserMapper externalUserMapper , IUserHttpMapper userHttpMapper
            , IHttpUserContextAccessor httpUserContextAccessor
            , ITokenFactory tokenFactory, ILogger<AuthService> logger
            , UserDbContext userContext)
        {
            _authOrchestrator = authOrchestrator;
            _authDomainService = authDomainService;
            _otpDomainService = otpDomainService;
            _userServiceClient = userServiceClient;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _otpRepository = otpRepository;
            _unitOfWork = unitOfWork;
            _externalAuthProvider = externalAuthProvider;
            _externalUserMapper = externalUserMapper;
            _userHttpMapper = userHttpMapper;
            _tokenFactory = tokenFactory;
            _httpUserContextAccessor = httpUserContextAccessor;
            _logger = logger;
            _userContext = userContext;
        }

        // Đăng nhập và tạo token cho người dùng
        public async Task<ResultDTO<GenerateTokenUserDTO>> LoginUserGenerateTokenAsync(LoginUserPasswordRequestDTO request ,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: LoginUserGenerateTokenAsync gọi với: {Account}", request.Account);
            
            var userClaims = await _authOrchestrator.FetchUserClaimsForLoginAsync(request, cancellationToken);

            // Tạo token
            var token = _authDomainService.GenerateToken(userClaims);

            var refreshToken = _authDomainService.GenarateRefreshToken();
            var refreshTokenEntity = _tokenFactory.CreateRefreshToken(refreshToken, userClaims.UserId);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var saveRefresh = await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(refreshTokenEntity);
            if (saveRefresh == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationService: LoginUserGenerateTokenAsync lưu mã refresh token thất bại với: {Account}", request.Account);
                throw new SaveRefreshTokenFailedException();
            }

            var result = new GenerateTokenUserDTO
            {
                Token = token.Value,
                RefreshToken = saveRefresh.Token
            };

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationService: LoginUserGenerateTokenAsync đăng nhập thành công với: {Account}", request.Account);
            return ResultDTO<GenerateTokenUserDTO>.Success(result, "Đăng nhập thành công");
        }

        // Refresh Token khi 
        public async Task<ResultDTO<GenerateTokenUserDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: RefreshTokenAsync bắt đầu với refreshToken = {RefreshToken}", request.RefreshToken);

            // Lấy refresh token từ DB
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("ApplicationService: RefreshTokenAsync không tìm thấy refresh token = {RefreshToken}", request.RefreshToken);
                throw new InvalidRefreshTokenException();
            }

            // Domain check: refresh token có còn hợp lệ không?
            _authDomainService.IsActiveRefreshToken(refreshToken);

            // Lấy thông tin user từ refresh token
            var userClaims = await _authOrchestrator.FetchUserClaimsByIdAsync(refreshToken.UserId, cancellationToken);

            // Sinh access token mới
            var newAccessToken = _authDomainService.GenerateToken(userClaims);

            var result = new GenerateTokenUserDTO
            {
                Token = newAccessToken.Value,
            };

            _logger.LogInformation("ApplicationService: RefreshTokenAsync thành công cho userId = {UserId}", refreshToken.UserId);
            return ResultDTO<GenerateTokenUserDTO>.Success(result, "Cấp lại token thành công");
        }

        // Thu hồi refresh token nội bộ
        private async Task<ResultDTO> RevokeInternalAsync(string refreshTokenValue, string successMsg, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ApplicationService: RevokeInternalAsync bắt đầu với refreshToken = {RefreshToken}", refreshTokenValue);
            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshTokenValue, cancellationToken);
            if (refreshToken == null)
            {
                _logger.LogWarning("ApplicationService: RevokeInternalAsync không tìm thấy refresh token = { RefreshToken}" , refreshTokenValue);
                throw new InvalidRefreshTokenException();
            }

            _authDomainService.RevokeRefreshToken(refreshToken);

            var updated = await _unitOfWork.RefreshTokens.UpdateRefreshTokenAsync(refreshToken);
            if (updated == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationService: RevokeInternalAsync thất bại khi revoke refresh token = {RefreshToken}", refreshTokenValue);
                throw new RevokeRefreshTokenFailedException();
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationService: RevokeInternalAsync thành công cho refreshToken = {RefreshToken}", refreshTokenValue);
            return ResultDTO.Success(successMsg);
        }

        // Đăng xuất và thu hồi refresh token
        public async Task<ResultDTO> LogoutAsync(LogOutRequestDTO request, CancellationToken cancellationToken = default)
        {
           return await RevokeInternalAsync(request.RefreshToken, "Đăng xuất thành công", cancellationToken);
        }

        // Thu hồi refresh token
        public async Task<ResultDTO> RevokeRefreshTokenAsync(RevokeRefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
        {
            return await RevokeInternalAsync(request.RefreshToken, "Thu hồi refresh token thành công", cancellationToken);
        }

        // Đăng nhập bằng phương thức bên ngoài sinh token
        public async Task<ResultDTO<GenerateTokenUserDTO>> LoginExternalGenerateTokenAsync(ExternalAuthTokenRequestDTO requestExternal, CancellationToken cancellationToken = default) // Nhà cung cấp , id user của bên cung cấp
        {
            _logger.LogInformation("ApplicationService: LoginExternalGenerateTokenAsync gọi với: {Provider}", requestExternal.Provider);
            // tìm nhà cung cấp
            var externalProvider = _externalAuthProvider.FirstOrDefault(p => p.ProviderName.Equals(requestExternal.Provider, StringComparison.OrdinalIgnoreCase)); // tìm không phân biệt chử hoa thường

            if (externalProvider == null)
            {
                _logger.LogWarning("ApplicationService: LoginExternalGenerateTokenAsync nhà cung cấp không hợp lệ: {Provider}", requestExternal.Provider);
                throw new InvalidAuthProviderException(requestExternal.Provider);
            }

            // xác thực token bên cung cấp
            var externalUser = await externalProvider.ValidateAsync(requestExternal.IdToken);
            if (externalUser == null)
            {
                _logger.LogWarning("ApplicationService: LoginExternalGenerateTokenAsync token không hợp lệ từ nhà cung cấp: {Provider}", requestExternal.Provider);
                throw new InvalidTokenException();
            }

            // ánh xạ sang DTO gửi đến UserService
            var request = _externalUserMapper.ToAuthDTO(externalUser);

            var claims = await _authOrchestrator.FetchUserClaimsFromExternalAsync(request, cancellationToken);

            // Tạo token
            var token = _authDomainService.GenerateToken(claims);

            // Tạo refresh token và lưu vào DB
            var refreshToken = _authDomainService.GenarateRefreshToken();
            var refreshTokenEntity = _tokenFactory.CreateRefreshToken(refreshToken, claims.UserId);

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var saveRefresh = await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(refreshTokenEntity);
            if (saveRefresh == null)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError("ApplicationService: LoginUserGenerateTokenAsync lưu mã refresh token thất bại với: {Account}", request.Email);
                throw new SaveRefreshTokenFailedException();
            }

            var result = new GenerateTokenUserDTO
            {
                Token = token.Value,
                RefreshToken = saveRefresh.Token
            };

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("ApplicationService: LoginExternalGenerateTokenAsync đăng nhập thành công với: {Account}", request.Email);
            return ResultDTO<GenerateTokenUserDTO>.Success(result, "Đăng nhập thành công");

        }

        // Gửi mã OTP đến email người dùng
        public async Task<ResultDTO> GenerateAndSendOtpEmailAsync(ForgotPasswordEmailRequestDTO forgot, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: bắt đầu tạo OTP cho user {Email}", forgot.Email);

            var userResponse = await _authOrchestrator.FetchUserByEmailAsync(forgot.Email, cancellationToken);

            if (userResponse.TypeLogin != "Local")
            {
                _logger.LogError("ApplicationService: không thể gửi OTP cho user {Email} vì đăng nhập bằng nhà cung cấp bên ngoài", forgot.Email);
                throw new SendOtpForgotForExternalUserFailedException(forgot.Email, userResponse.TypeLogin);
            }

            var otpCode = _otpDomainService.CreateOtp(userResponse.UserId, forgot.Type);
            await _otpRepository.SaveAsync(otpCode, cancellationToken);

            var subject = _emailTemplateService.GetOtpSubject(forgot.Type);
            var body = _emailTemplateService.GetOtpBody(otpCode.Code);

            await _emailService.SendAsync(userResponse.Email,subject,body);

            _logger.LogInformation("ApplicationService: đã gửi OTP qua email {Email}", userResponse.Email);
            return ResultDTO.Success($"OTP đã được gửi đến: {userResponse.Email}");
        }

        // Xác thực mã OTP cho việc quên mật khẩu
        public async Task<ResultDTO> VerifyForgotPasswordOtpAsync(VerifyOtpEmailRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: bắt đầu xác thực OTP cho user {Email}", request.Email);

            var userResponse = await _authOrchestrator.FetchUserByEmailAsync(request.Email, cancellationToken);

            var codeOtp = await _otpRepository.GetAsync(userResponse.UserId, request.Type, cancellationToken);

            var valid = _otpDomainService.VerifyOtp(codeOtp,request.Otp);

            _logger.LogInformation("ApplicationService: OTP hợp lệ cho user {Email}", request.Email);
            return ResultDTO.Success("Xác thực OTP thành công");

        }

        // Xác thực mã OTP và đặt lại mật khẩu
        public async Task<ResultDTO> VerifyOtpForResetPasswordAsync(VerifyOtpEmailResetPasswordRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: bắt đầu xác thực OTP cho user {Email}", request.Email);
            // Kiểm tra mật khẩu mới và xác nhận mật khẩu mới có khớp không
            var userResponse = await _authOrchestrator.FetchUserByEmailAsync(request.Email, cancellationToken);

            var codeOtp = await _otpRepository.GetAsync(userResponse.UserId, request.Type, cancellationToken);

            var valid = _otpDomainService.VerifyOtp(codeOtp, request.Otp);
            await _otpRepository.RemoveAsync(codeOtp.UserId, codeOtp.Type); // Xóa OTP sau khi xác thực thành công

            var resetRequest = _userHttpMapper.ToResetPasswordDTO(request, userResponse.UserId);

            // Đặt lại mật khẩu
            var result = await _authOrchestrator.ResetPasswordAsync(resetRequest, cancellationToken);

            _logger.LogInformation("Orchestrator: OTP hợp lệ cho user {Email}", request.Email);
            return ResultDTO.Success(result.Msg);
        }

        // Gửi mã OTP đến email người dùng để thay đổi email
        public async Task<ResultDTO> GenerateAndSendOtpChangeEmailAsync(ChangeEmailOtpRequestDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: bắt đầu tạo OTP thay đổi email ");

            var userId = _httpUserContextAccessor.GetUserId();
            var email = _httpUserContextAccessor.GetEmail();

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("ApplicationService: Không tìm thấy userId từ token");
                throw new UserNotFoundByIdException(userId);
            }
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("ApplicationService: Không tìm thấy email từ token");
                throw new UserEmailNotFoundException(email); // Nếu bạn có custom exception cho email
            }

            // Tạo mã OTP
            var otpCode = _otpDomainService.CreateOtp(userId, request.Type);
            await _otpRepository.SaveAsync(otpCode, cancellationToken);

            // Gửi email
            var subject = _emailTemplateService.GetOtpSubject(request.Type);
            var body = _emailTemplateService.GetOtpBody(otpCode.Code);
            await _emailService.SendAsync(email, subject, body);

            _logger.LogInformation("ApplicationService: đã gửi OTP qua email {Email}", email);
            return ResultDTO.Success($"OTP change Email đã được gửi đến: {email}");

        }

        // Xác thực mã OTP và thay đổi email
        public async Task<ResultDTO> VerifyOtpForChangeEmailAsync(VerifyChangeEmailRequestDTO request , CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("ApplicationService: bắt đầu xác thực OTP và thay đổi email mới: {NewEmail}", request.NewEmail);

            var userId = _httpUserContextAccessor.GetUserId();
            if(string.IsNullOrEmpty(userId))
            {
                _logger.LogError("ApplicationService: Không tìm thấy userId từ token");
                throw new UserNotFoundByIdException(userId);
            }

            var codeOtp = await _otpRepository.GetAsync(userId, request.Type);
            var valid = _otpDomainService.VerifyOtp(codeOtp, request.Otp);

            await _otpRepository.RemoveAsync(codeOtp.UserId, codeOtp.Type); // Xóa OTP sau khi xác thực thành công


            var mapRequest = _userHttpMapper.ToChangEmailDTO(request, userId);
            var result = await _authOrchestrator.ChangeEmailAsync(mapRequest, cancellationToken);

            _logger.LogInformation("ApplicationService: thay đổi email thành công cho userId {UserId} với email mới {NewEmail}", userId, request.NewEmail);
            return ResultDTO.Success(result.Msg);
        }

        public async Task<ResultListDTO<List<RefreshTokenInfoDTO>>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            // 🔹 Lấy danh sách RefreshToken phân trang
            var (tokens, total, last) = await _unitOfWork.RefreshTokens
                .GetAllAsync(page, limit, cancellationToken);

            // 🔹 Lấy danh sách UserId duy nhất để join
            var userIds = tokens.Select(t => t.UserId).Distinct().ToList();

            // 🔹 Join với bảng User để lấy Email
            var users = await _userContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync(cancellationToken);

            // 🔹 Map entity -> DTO
            var items = tokens.Select(t =>
            {
                var user = users.FirstOrDefault(u => u.Id == t.UserId);
                return new RefreshTokenInfoDTO
                {
                    UserId = t.UserId,
                    Email = user?.Email ?? "(Không tìm thấy)",
                    Token = t.Token,
                    RevokedAt = t.RevokedAt
                };
            }).ToList();

            // 🔹 Pagination
            var pagination = new PaginationDTO
            {
                Page = page,
                Limit = limit,
                Total = total,
                Last = last
            };

            // ✅ Trả kết quả
            return ResultListDTO<List<RefreshTokenInfoDTO>>.Success(
                items,
                "Lấy danh sách RefreshToken thành công!",
                pagination
            );
        }

    }
}
