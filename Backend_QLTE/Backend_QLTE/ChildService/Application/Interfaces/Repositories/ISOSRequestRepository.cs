using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface ISOSRequestRepository
    {
        // 🟢 Lấy tất cả yêu cầu SOS có phân trang
        Task<(List<SOSRequest> sosRequests, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);

        // 🟡 Lấy 1 yêu cầu SOS theo Id
        Task<SOSRequest?> GetByIdAsync(Guid sosId, CancellationToken cancellationToken = default);

        // 🔵 Lấy danh sách yêu cầu SOS của 1 đứa trẻ cụ thể
        Task<List<SOSRequest>> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default);

        // 🟣 Lấy danh sách yêu cầu SOS của 1 đứa trẻ có phân trang
        Task<(List<SOSRequest> sosRequests, int total, int last)> GetByChildPagedAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default);

        // 🟢 Tạo mới
        Task<SOSRequest> CreateAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default);

        // 🟡 Cập nhật
        Task<SOSRequest> UpdateAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default);

        // 🔴 Xóa
        Task DeleteAsync(SOSRequest sosRequest, CancellationToken cancellationToken = default);
    }
}
