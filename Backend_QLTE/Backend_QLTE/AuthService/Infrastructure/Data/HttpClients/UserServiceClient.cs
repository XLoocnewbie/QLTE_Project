using Backend_QLTE.AuthService.Application.DTOs.Common;
using Backend_QLTE.AuthService.Application.DTOs.ForgotPassword;
using Backend_QLTE.AuthService.Application.DTOs.Login;
using Backend_QLTE.AuthService.Application.DTOs.User;
using Backend_QLTE.AuthService.Infrastructure.Data.HttpClients.Interfaces;


namespace Backend_QLTE.AuthService.Infrastructure.Data.HttpClients
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;
        public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        // Ví dụ: gọi UserService để lấy thông tin user theo email
        public async Task<ApiResponse<LoginTokenResponseDTO>?> LoginUserAsync(LoginUserPasswordRequestDTO request,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để đăng nhập user với tài khoản: {Account}", request.Account);
            var response = await _httpClient.PostAsJsonAsync("api/User/LoginLocalUser", request, cancellationToken);

            // Đọc body JSON
            var body = await response.Content.ReadFromJsonAsync<LoginTokenResponseDTO>(
                cancellationToken: cancellationToken);

            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            // Trả về wrapper có cả status code
            return new ApiResponse<LoginTokenResponseDTO>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }

        // Người dùng đăng nhập bằng nhà cung cấp bên ngoài Cho token 'Google,FaceBook,...'
        public async Task<ApiResponse<LoginTokenResponseDTO>?> ProviderLoginUserAsync(ExternalAuthUserInfoDTO request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để đăng nhập hoặc đăng ký user với nhà cung cấp: {Provider}", request.TypeLogin);
            var response = await _httpClient.PostAsJsonAsync("api/User/ProviderLoginOrRegisterUser", request,cancellationToken);

            // Đọc body JSON
            var body = await response.Content.ReadFromJsonAsync<LoginTokenResponseDTO>(
                cancellationToken: cancellationToken);

            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            // Trả về wrapper có cả status code
            return new ApiResponse<LoginTokenResponseDTO>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }

        // Tìm User theo email
        public async Task<ApiResponse<ResultDTO<UserResponseDTO>>?> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để lấy thông tin user với email: {Email}", email);
            var response = await _httpClient.GetAsync($"api/User/GetUserByEmail?email={email}", cancellationToken);
            // Đọc body JSON
            var body = await response.Content.ReadFromJsonAsync<ResultDTO<UserResponseDTO>>(
                cancellationToken: cancellationToken);

            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            // Trả về wrapper có cả status code
            return new ApiResponse<ResultDTO<UserResponseDTO>>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }

        public async Task<ApiResponse<ResultDTO<UserResponseDTO>>?> FindUserByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để lấy thông tin user với UserId: {UserId}", userId);
            var response = await _httpClient.GetAsync($"api/User/GetUserByUserId?userId={userId}", cancellationToken);
            // Đọc body JSON
            var body = await response.Content.ReadFromJsonAsync<ResultDTO<UserResponseDTO>>(
                cancellationToken: cancellationToken);
            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            // Trả về wrapper có cả status code
            return new ApiResponse<ResultDTO<UserResponseDTO>>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }

        // Đặt lại mật khẩu cho user
        public async Task<ApiResponse<ResultDTO>> ResetPasswordAsync(ResetPasswordEmailHttpRequestDTO requet,CancellationToken cancellationToken = default) 
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để đặt lại mật khẩu cho user với UserId: {UserId}", requet.UserId);
            var response = await _httpClient.PostAsJsonAsync("api/User/ResetPassword", requet, cancellationToken);

            var body = await response.Content.ReadFromJsonAsync<ResultDTO>(
                cancellationToken: cancellationToken);

            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            return new ApiResponse<ResultDTO>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }

        // Đổi email cho user
        public async Task<ApiResponse<ResultDTO>> ChangeEmailAsync(ChangeEmailHttpRequestDTO requestDTO , CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("UserServiceClient: Gọi API UserService để đổi email cho user với UserId: {UserId}", requestDTO.UserId);
            var response = await _httpClient.PostAsJsonAsync("api/User/ChangeEmail", requestDTO, cancellationToken);

            var body = await response.Content.ReadFromJsonAsync<ResultDTO>(
                cancellationToken: cancellationToken);

            _logger.LogInformation("UserServiceClient: API UserService trả về mã trạng thái: {StatusCode}", (int)response.StatusCode);
            return new ApiResponse<ResultDTO>
            {
                Data = body,
                StatusCode = (int)response.StatusCode
            };
        }
    }
}
