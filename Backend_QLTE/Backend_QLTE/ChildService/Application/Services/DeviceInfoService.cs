using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Application.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class DeviceInfoService : IDeviceInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceInfoOrchestrator _orchestrator;
        private readonly IDeviceInfoResponseMapper _mapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly ILogger<DeviceInfoService> _logger;

        public DeviceInfoService(
            IUnitOfWork unitOfWork,
            IDeviceInfoOrchestrator orchestrator,
            IDeviceInfoResponseMapper mapper,
            IPaginationMapper paginationMapper,
            IHubContext<DeviceHub> hubContext,
            ILogger<DeviceInfoService> logger)
        {
            _unitOfWork = unitOfWork;
            _orchestrator = orchestrator;
            _mapper = mapper;
            _paginationMapper = paginationMapper;
            _hubContext = hubContext;
            _logger = logger;
        }

        private async Task BroadcastToGroup(Guid childId, string eventName, object payload)
        {
            await _hubContext.Clients.Group(childId.ToString()).SendAsync(eventName, payload);
            _logger.LogInformation("SignalR: Gửi event {EventName} tới ChildId={ChildId}", eventName, childId);
        }

        // 🟢 Lấy tất cả thiết bị
        public async Task<ResultListDTO<DeviceInfoResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var (entities, total, last) = await _unitOfWork.DeviceInfos.GetAllAsync(page, limit, cancellationToken);
            if (total == 0)
                throw new DeviceInfoNotFoundException();

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);
            return ResultListDTO<DeviceInfoResponseDTO>.Success(dtoList, "Lấy danh sách thiết bị thành công", pagination);
        }

        // 🔵 Lấy chi tiết
        public async Task<ResultDTO<DeviceInfoResponseDTO>> GetDetailAsync(Guid deviceId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.DeviceInfos.GetByIdAsync(deviceId, cancellationToken)
                ?? throw new DeviceInfoNotFoundException(deviceId);

            var dto = _mapper.ToDto(entity);
            return ResultDTO<DeviceInfoResponseDTO>.Success(dto, "Lấy chi tiết thiết bị thành công");
        }

        // 🟡 Lấy thiết bị theo ChildId
        public async Task<ResultDTO<DeviceInfoResponseDTO>> GetByChildAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.DeviceInfos.GetByChildIdAsync(childId, cancellationToken)
                ?? throw new DeviceInfoNotFoundException();

            return ResultDTO<DeviceInfoResponseDTO>.Success(_mapper.ToDto(entity), "Lấy thiết bị theo trẻ thành công");
        }

        // 🟢 Tạo mới
        public async Task<ResultDTO> CreateAsync(DeviceInfoCreateDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.DeviceInfos.ExistsByIMEIAsync(dto.IMEI, cancellationToken))
                throw new DuplicateDeviceInfoException(dto.IMEI);

            var created = await _orchestrator.CreateAsync(dto, userId, cancellationToken);
            await BroadcastToGroup(dto.ChildId, "DeviceCreated", _mapper.ToDto(created));

            return ResultDTO.Success("Tạo thiết bị thành công");
        }

        // 🟠 Cập nhật
        public async Task<ResultDTO<DeviceInfoResponseDTO>> UpdateAsync(DeviceInfoUpdateDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            var updated = await _orchestrator.UpdateAsync(dto, userId, cancellationToken);
            var response = _mapper.ToDto(updated);

            await BroadcastToGroup(dto.ChildId, "DeviceUpdated", response);
            return ResultDTO<DeviceInfoResponseDTO>.Success(response, "Cập nhật thiết bị thành công");
        }

        // 🔴 Xóa
        public async Task<ResultDTO> DeleteAsync(DeviceInfoDeleteDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.DeviceInfos.GetByIdAsync(dto.DeviceId, cancellationToken)
                ?? throw new DeviceInfoNotFoundException(dto.DeviceId);

            await _orchestrator.DeleteAsync(dto, userId, cancellationToken);
            await BroadcastToGroup(entity.ChildId, "DeviceDeleted", new { dto.DeviceId });

            return ResultDTO.Success("Xóa thiết bị thành công");
        }

        // ⚡ Cập nhật trạng thái pin / online (giữ nguyên - monitoring)
        public async Task<ResultDTO> UpdateStatusAsync(Guid deviceId, int? pin, bool? online, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.DeviceInfos.GetByIdAsync(deviceId, cancellationToken)
                ?? throw new DeviceInfoNotFoundException(deviceId);

            if (pin.HasValue) entity.Pin = pin.Value;
            if (online.HasValue) entity.TrangThaiOnline = online.Value;
            entity.LanCapNhatCuoi = DateTime.Now;

            await _unitOfWork.DeviceInfos.UpdateAsync(entity, cancellationToken);

            await BroadcastToGroup(entity.ChildId, "DeviceStatusUpdated", new
            {
                entity.DeviceId,
                entity.Pin,
                entity.TrangThaiOnline,
                entity.LanCapNhatCuoi
            });

            return ResultDTO.Success("Cập nhật trạng thái thiết bị thành công");
        }

        // 🔒 Khoá thiết bị
        public async Task<ResultDTO> LockDeviceAsync(Guid childId, string userId, CancellationToken cancellationToken = default)
        {
            await _orchestrator.SetLockStateAsync(childId, true, userId, cancellationToken);
            await BroadcastToGroup(childId, "DeviceLocked", new { ChildId = childId, LockedBy = userId });

            return ResultDTO.Success("Thiết bị đã bị khoá thành công (realtime).");
        }

        // 🔓 Mở khoá thiết bị
        public async Task<ResultDTO> UnlockDeviceAsync(Guid childId, string userId, CancellationToken cancellationToken = default)
        {
            await _orchestrator.SetLockStateAsync(childId, false, userId, cancellationToken);
            await BroadcastToGroup(childId, "DeviceUnlocked", new { ChildId = childId, UnlockedBy = userId });

            return ResultDTO.Success("Thiết bị đã được mở khoá thành công (realtime).");
        }

        // 🆕 Bật/tắt theo dõi định kỳ
        public async Task<ResultDTO> SetTrackingStateAsync(Guid childId, bool isTracking, string userId, CancellationToken cancellationToken = default)
        {
            await _orchestrator.SetTrackingStateAsync(childId, isTracking, userId, cancellationToken);
            await BroadcastToGroup(childId, "DeviceTrackingChanged", new { ChildId = childId, IsTracking = isTracking });

            return ResultDTO.Success(isTracking
                ? "Bắt đầu theo dõi thiết bị của trẻ."
                : "Đã tạm dừng theo dõi thiết bị của trẻ.");
        }
    }
}
