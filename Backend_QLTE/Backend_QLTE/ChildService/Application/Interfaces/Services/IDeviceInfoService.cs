using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.DTOs.Common;

namespace Backend_QLTE.ChildService.Application.Interfaces.Services
{
    public interface IDeviceInfoService
    {
        // 🟢 Lấy danh sách thiết bị (phân trang)
        Task<ResultListDTO<DeviceInfoResponseDTO>> GetAllAsync(
            int page, int limit, CancellationToken cancellationToken = default);

        // 🔵 Lấy chi tiết thiết bị
        Task<ResultDTO<DeviceInfoResponseDTO>> GetDetailAsync(
            Guid deviceId, CancellationToken cancellationToken = default);

        // 🟡 Lấy thiết bị theo trẻ (ChildId)
        Task<ResultDTO<DeviceInfoResponseDTO>> GetByChildAsync(
            Guid childId, CancellationToken cancellationToken = default);

        // 🟢 Tạo mới thiết bị
        Task<ResultDTO> CreateAsync(
            DeviceInfoCreateDTO dto, string userId, CancellationToken cancellationToken = default);

        // 🟠 Cập nhật thiết bị
        Task<ResultDTO<DeviceInfoResponseDTO>> UpdateAsync(
            DeviceInfoUpdateDTO dto, string userId, CancellationToken cancellationToken = default);

        // 🔴 Xóa thiết bị
        Task<ResultDTO> DeleteAsync(
            DeviceInfoDeleteDTO dto, string userId, CancellationToken cancellationToken = default);

        // ⚡ Cập nhật trạng thái pin / online (Realtime)
        Task<ResultDTO> UpdateStatusAsync(
            Guid deviceId, int? pin, bool? online, CancellationToken cancellationToken = default);

        // 🔒 Khoá thiết bị (update IsLocked = true + realtime)
        Task<ResultDTO> LockDeviceAsync(
            Guid childId, string userId, CancellationToken cancellationToken = default);

        // 🔓 Mở khoá thiết bị (update IsLocked = false + realtime)
        Task<ResultDTO> UnlockDeviceAsync(
            Guid childId, string userId, CancellationToken cancellationToken = default);

        // 🆕 Bật/tắt theo dõi định kỳ (IsTracking)
        Task<ResultDTO> SetTrackingStateAsync(
            Guid childId, bool isTracking, string userId, CancellationToken cancellationToken = default);
    }
}
