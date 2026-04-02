using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceInfo;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Backend_QLTE.ChildService.Infrastructure.Data.HttpClients.Interfaces;
using Backend_QLTE.ChildService.Domain.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Domain.Exceptions.NotFounds;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Orchestrators
{
    public class DeviceInfoOrchestrator : IDeviceInfoOrchestrator
    {
        private readonly IDeviceInfoFactory _factory;
        private readonly IDeviceInfoDomainService _domainService;
        private readonly IDeviceInfoRepository _repository;
        private readonly IChildRepository _childRepository;
        private readonly IUserServiceClient _userServiceClient;
        private readonly ILogger<DeviceInfoOrchestrator> _logger;

        public DeviceInfoOrchestrator(
            IDeviceInfoFactory factory,
            IDeviceInfoDomainService domainService,
            IDeviceInfoRepository repository,
            IChildRepository childRepository,
            IUserServiceClient userServiceClient,
            ILogger<DeviceInfoOrchestrator> logger)
        {
            _factory = factory;
            _domainService = domainService;
            _repository = repository;
            _childRepository = childRepository;
            _userServiceClient = userServiceClient;
            _logger = logger;
        }

        // 🟢 Tạo mới DeviceInfo
        public async Task<DeviceInfo> CreateAsync(DeviceInfoCreateDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu tạo DeviceInfo cho ChildId={ChildId}, UserId={UserId}", dto.ChildId, userId);

            var userResponse = await _userServiceClient.FindUserByUserIdAsync(userId, cancellationToken);
            if (userResponse?.Data == null || !userResponse.Data.Status)
                throw new KeyNotFoundException($"Không tìm thấy phụ huynh với UserId={userId}");

            var child = await _childRepository.GetByIdAsync(dto.ChildId, cancellationToken);
            if (child == null)
                throw new ChildNotFoundException(dto.ChildId.ToString());
            if (child.UserId != userId)
                throw new UnauthorizedAccessException("Không có quyền thêm thiết bị cho trẻ này.");

            var exists = await _repository.ExistsByIMEIAsync(dto.IMEI, cancellationToken);
            if (exists)
                throw new DuplicateDeviceInfoException(dto.IMEI);

            var entity = _factory.Create(dto);
            _domainService.EnsureCanCreate(entity);

            var created = await _repository.CreateAsync(entity, cancellationToken);
            _logger.LogInformation("Orchestrator: DeviceInfo {Id} đã được tạo thành công cho ChildId={ChildId}", created.DeviceId, dto.ChildId);

            return created;
        }

        // 🟡 Cập nhật DeviceInfo
        public async Task<DeviceInfo> UpdateAsync(DeviceInfoUpdateDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu cập nhật DeviceInfo Id={DeviceId} cho UserId={UserId}", dto.DeviceId, userId);

            var existing = await _repository.GetByIdAsync(dto.DeviceId, cancellationToken);
            if (existing == null)
                throw new DeviceInfoNotFoundException(dto.DeviceId.ToString());

            var child = await _childRepository.GetByIdAsync(dto.ChildId, cancellationToken);
            if (child == null)
                throw new ChildNotFoundException(dto.ChildId.ToString());
            if (child.UserId != userId)
                throw new UnauthorizedAccessException("Không có quyền chỉnh sửa thiết bị của trẻ này.");

            var updatedEntity = _factory.Update(dto);
            _domainService.EnsureCanUpdate(updatedEntity);

            var updated = await _repository.UpdateAsync(updatedEntity, cancellationToken);
            _logger.LogInformation("Orchestrator: DeviceInfo Id={DeviceId} cập nhật thành công.", dto.DeviceId);

            return updated;
        }

        // 🔴 Xóa DeviceInfo
        public async Task DeleteAsync(DeviceInfoDeleteDTO dto, string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu xóa DeviceInfo Id={DeviceId} cho UserId={UserId}", dto.DeviceId, userId);

            var entity = await _repository.GetByIdAsync(dto.DeviceId, cancellationToken);
            if (entity == null)
                throw new DeviceInfoNotFoundException(dto.DeviceId.ToString());

            var child = await _childRepository.GetByIdAsync(entity.ChildId, cancellationToken);
            if (child == null)
                throw new ChildNotFoundException(entity.ChildId.ToString());
            if (child.UserId != userId)
                throw new UnauthorizedAccessException("Không có quyền xóa thiết bị của trẻ này.");

            _domainService.EnsureCanDelete(entity);

            await _repository.DeleteAsync(entity, cancellationToken);
            _logger.LogInformation("Orchestrator: DeviceInfo Id={DeviceId} đã được xóa thành công.", dto.DeviceId);
        }

        // 🆕 Bật/tắt theo dõi thiết bị (IsTracking)
        public async Task<bool> SetTrackingStateAsync(Guid childId, bool isTracking, string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Đặt trạng thái theo dõi={IsTracking} cho ChildId={ChildId}", isTracking, childId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
                throw new ChildNotFoundException(childId.ToString());
            if (child.UserId != userId)
                throw new UnauthorizedAccessException("Không có quyền thay đổi theo dõi cho trẻ này.");

            var device = await _repository.GetByChildIdAsync(childId, cancellationToken);
            if (device == null)
                throw new DeviceInfoNotFoundException($"Không tìm thấy thiết bị của trẻ {childId}");

            device.IsTracking = isTracking;
            device.LanCapNhatCuoi = DateTime.Now;

            await _repository.UpdateAsync(device, cancellationToken);
            _logger.LogInformation("Orchestrator: Thiết bị {DeviceId} đã được {Action} theo dõi.",
                device.DeviceId, isTracking ? "bật" : "tắt");

            return true;
        }

        // 🆕 Khoá/Mở khoá thiết bị (IsLocked)
        public async Task<bool> SetLockStateAsync(Guid childId, bool isLocked, string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Thực hiện {Action} thiết bị của ChildId={ChildId}", isLocked ? "🔒 khoá" : "🔓 mở khoá", childId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
                throw new ChildNotFoundException(childId.ToString());
            if (child.UserId != userId)
                throw new UnauthorizedAccessException("Không có quyền thay đổi khoá cho trẻ này.");

            var success = await _repository.SetLockStateAsync(childId, isLocked, cancellationToken);
            if (!success)
                throw new DeviceInfoNotFoundException($"Không tìm thấy thiết bị của trẻ {childId}");

            _logger.LogInformation("Orchestrator: Thiết bị của ChildId={ChildId} đã được {Action}.",
                childId, isLocked ? "khoá" : "mở khoá");

            return true;
        }
    }
}
