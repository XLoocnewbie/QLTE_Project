using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.DTOs.Client.User;

namespace Backend_QLTE.ChildService.Infrastructure.Data.HttpClients.Interfaces
{
    public interface IUserServiceClient
    {
        Task<ApiResponse<ResultDTO<UserHttpResponseDTO>>?> FindUserByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
