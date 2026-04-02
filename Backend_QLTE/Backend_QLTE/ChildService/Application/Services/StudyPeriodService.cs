using Microsoft.EntityFrameworkCore;
using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Application.Exceptions.Failed;
using Backend_QLTE.ChildService.Application.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Backend_QLTE.Hubs;
using Backend_QLTE.ChildService.Infrastructure.Data;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class StudyPeriodService : IStudyPeriodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStudyPeriodOrchestrator _orchestrator;
        private readonly IStudyPeriodResponseMapper _mapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly ILogger<StudyPeriodService> _logger;
        private readonly IHubContext<StudyHub> _hubContext;
        private readonly ChildDbContext _childContext;
        private readonly IDeviceRestrictionService _restrictionService;

        public StudyPeriodService(
            IUnitOfWork unitOfWork,
            IStudyPeriodOrchestrator orchestrator,
            IStudyPeriodResponseMapper mapper,
            IPaginationMapper paginationMapper,
            ILogger<StudyPeriodService> logger,
            IHubContext<StudyHub> hubContext,
            ChildDbContext childContext,
            IDeviceRestrictionService restrictionService)
        {
            _unitOfWork = unitOfWork;
            _orchestrator = orchestrator;
            _mapper = mapper;
            _paginationMapper = paginationMapper;
            _logger = logger;
            _hubContext = hubContext;
            _childContext = childContext;
            _restrictionService = restrictionService;
        }

        // 🟢 Lấy danh sách có phân trang
        public async Task<ResultListDTO<StudyPeriodResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetAllAsync với trang {Page}, giới hạn {Limit}", page, limit);

            var (entities, total, last) = await _unitOfWork.StudyPeriods.GetAllAsync(page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không tìm thấy StudyPeriod nào");
                throw new StudyPeriodNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);

            return ResultListDTO<StudyPeriodResponseDTO>.Success(dtoList, "Lấy danh sách StudyPeriod thành công", pagination);
        }

        // 🔵 Lấy chi tiết theo Id
        public async Task<ResultDTO<StudyPeriodResponseDTO>> GetDetailAsync(Guid studyPeriodId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetDetailAsync với Id={Id}", studyPeriodId);

            var entity = await _unitOfWork.StudyPeriods.GetByIdAsync(studyPeriodId, cancellationToken);
            if (entity is null)
            {
                _logger.LogWarning("Service: Không tìm thấy StudyPeriod Id={Id}", studyPeriodId);
                throw new StudyPeriodNotFoundException(studyPeriodId);
            }

            var dto = _mapper.ToDto(entity);
            return ResultDTO<StudyPeriodResponseDTO>.Success(dto, "Lấy chi tiết StudyPeriod thành công");
        }

        public async Task<ResultListDTO<StudyPeriodResponseDTO>> GetAllStudyPeriodsByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetAllStudyPeriodsByChildAsync với ChildId={ChildId}", childId);

            var (entities, total, last) = await _unitOfWork.StudyPeriods.GetByChildPagedAsync(childId, page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không tìm thấy StudyPeriod cho ChildId={ChildId}", childId);
                throw new StudyPeriodNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);

            return ResultListDTO<StudyPeriodResponseDTO>.Success(dtoList, "Lấy danh sách khung giờ học theo trẻ thành công", pagination);
        }

        // 🟡 Tạo mới
        public async Task<ResultDTO> CreateAsync(StudyPeriodCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu CreateAsync cho ChildId={ChildId}", dto.ChildId);

            var existingPeriods = await _unitOfWork.StudyPeriods.GetByChildIdAsync(dto.ChildId, cancellationToken);
            if (existingPeriods.Any(p => dto.StartTime < p.EndTime && dto.EndTime > p.StartTime))
                throw new DuplicateStudyPeriodException(dto.StartTime, dto.EndTime);

            await _orchestrator.CreateAsync(dto, cancellationToken);

            _logger.LogInformation("Service: Tạo StudyPeriod cho ChildId={ChildId} thành công", dto.ChildId);
            return ResultDTO.Success("Tạo StudyPeriod thành công");
        }

        // 🟠 Cập nhật
        public async Task<ResultDTO<StudyPeriodResponseDTO>> UpdateAsync(StudyPeriodUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu UpdateAsync cho StudyPeriod Id={Id}", dto.StudyPeriodId);

            var updatedEntity = await _orchestrator.UpdateAsync(dto, cancellationToken);
            var response = _mapper.ToDto(updatedEntity);

            return ResultDTO<StudyPeriodResponseDTO>.Success(response, "Cập nhật StudyPeriod thành công");
        }

        // 🔴 Xóa
        public async Task<ResultDTO> DeleteAsync(StudyPeriodDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu DeleteAsync cho StudyPeriod Id={Id}", dto.StudyPeriodId);

            await _orchestrator.DeleteAsync(dto, cancellationToken);

            _logger.LogInformation("Service: Xoá StudyPeriod Id={Id} thành công", dto.StudyPeriodId);
            return ResultDTO.Success("Xóa StudyPeriod thành công");
        }

        public async Task<ResultDTO> ToggleActiveAsync(Guid studyPeriodId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: ToggleActiveAsync cho StudyPeriod Id={Id}", studyPeriodId);

            var entity = await _unitOfWork.StudyPeriods.GetByIdAsync(studyPeriodId, cancellationToken);
            if (entity is null)
            {
                _logger.LogWarning("Service: Không tìm thấy StudyPeriod Id={Id}", studyPeriodId);
                throw new StudyPeriodNotFoundException(studyPeriodId);
            }

            entity.IsActive = !entity.IsActive;

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.StudyPeriods.UpdateAsync(entity, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // 🔹 Lấy DeviceId của Child
            var device = await _childContext.DeviceInfos
                .FirstOrDefaultAsync(d => d.ChildId == entity.ChildId, cancellationToken);

            if (device == null)
            {
                _logger.LogWarning("⚠️ Không tìm thấy DeviceInfo cho ChildId={ChildId}", entity.ChildId);
                return ResultDTO.Fail("Không tìm thấy DeviceInfo cho trẻ này.");
            }

            // 🔹 Gọi DeviceRestrictionService để lấy danh sách chặn
            var restrictionResult = await _restrictionService.GetByDeviceIdAsync(device.DeviceId);
            List<string> blockedApps = new();

            if (restrictionResult.Status && restrictionResult.Data != null)
            {
                var studyRestriction = restrictionResult.Data
                    .FirstOrDefault(r => r.Mode == "StudyMode");

                if (studyRestriction != null && !string.IsNullOrEmpty(studyRestriction.BlockedApps))
                {
                    blockedApps = studyRestriction.BlockedApps
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .ToList();
                }
                else
                {
                    _logger.LogInformation("ℹ️ Không tìm thấy cấu hình StudyMode cho device-{DeviceId}", device.DeviceId);
                }
            }
            else
            {
                _logger.LogWarning("⚠️ Không có dữ liệu cấu hình hạn chế cho DeviceId={DeviceId}", device.DeviceId);
            }

            // ✅ Gửi sự kiện realtime qua SignalR (báo cho child)
            var payload = new
            {
                StudyPeriodId = entity.StudyPeriodId,
                ChildId = entity.ChildId,
                IsActive = entity.IsActive,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                BlockedApps = blockedApps,
                Message = entity.IsActive
                    ? "Thiết bị đang trong giờ học, các ứng dụng giải trí sẽ bị chặn."
                    : "Giờ học đã kết thúc, bạn có thể sử dụng thiết bị."
            };

            await _hubContext.Clients.Group(entity.ChildId.ToString())
                .SendAsync("OnStudyPeriodChanged", payload, cancellationToken);

            // 🟣 Gọi RestrictionService song song để bật/tắt restriction vật lý
            try
            {
                if (entity.IsActive)
                {
                    await _restrictionService.ActivateStudyRestrictionAsync(device.DeviceId);
                    _logger.LogInformation("🔒 Đã bật Restriction cho device-{DeviceId}", device.DeviceId);
                }
                else
                {
                    await _restrictionService.DeactivateRestrictionAsync(device.DeviceId);
                    _logger.LogInformation("🔓 Đã tắt Restriction cho device-{DeviceId}", device.DeviceId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi gọi RestrictionService từ StudyPeriodService (StudyPeriod Id={Id})", studyPeriodId);
            }

            var message = entity.IsActive ? "Đã bật khung giờ học" : "Đã tắt khung giờ học";
            _logger.LogInformation("Service: {Message} Id={Id}", message, studyPeriodId);
            return ResultDTO.Success(message);
        }

        // 🟣 Lấy khung giờ học đang hoạt động hiện tại theo ChildId
        public async Task<ResultDTO<StudyPeriodResponseDTO>> GetActiveByChildAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Kiểm tra khung giờ học đang hoạt động cho ChildId={ChildId}", childId);

            var entity = await _unitOfWork.StudyPeriods.GetActiveByChildAsync(childId, cancellationToken);
            if (entity == null)
            {
                _logger.LogInformation("Service: Không có khung giờ học đang hoạt động cho ChildId={ChildId}", childId);
                return ResultDTO<StudyPeriodResponseDTO>.Fail("Không có khung giờ học đang hoạt động.");
            }

            var dto = _mapper.ToDto(entity);
            _logger.LogInformation("Service: Đang trong khung giờ học {Start}–{End} cho ChildId={ChildId}",
                entity.StartTime, entity.EndTime, childId);

            return ResultDTO<StudyPeriodResponseDTO>.Success(dto, "Đang trong khung giờ học.");
        }
    }
}
