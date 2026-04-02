using Backend_QLTE.ChildService.Domain.Entities;

namespace Backend_QLTE.ChildService.Application.Interfaces.Repositories
{
    public interface IChildRepository
    {
        // 🟢 Lấy danh sách tất cả trẻ (phân trang)
        Task<(List<Child> children, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default);

        // 🟡 Lấy thông tin 1 trẻ theo Id
        Task<Child?> GetByIdAsync(Guid childId, CancellationToken cancellationToken = default);

        // 🔵 Lấy danh sách trẻ của một phụ huynh
        Task<List<Child>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        // 🟢 Tạo mới 1 trẻ
        Task<Child> CreateAsync(Child child, CancellationToken cancellationToken = default);

        // 🟡 Cập nhật thông tin trẻ
        Task<Child> UpdateAsync(Child child, CancellationToken cancellationToken = default);

        // 🔴 Xoá trẻ
        Task DeleteAsync(Child child, CancellationToken cancellationToken = default);
    }
}
