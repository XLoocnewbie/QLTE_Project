using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Application.Exceptions.Failed;
using Backend_QLTE.ChildService.Application.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Backend_QLTE.Hubs;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class SOSRequestService : ISOSRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISOSRequestOrchestrator _orchestrator;
        private readonly ISOSRequestResponseMapper _mapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly ILogger<SOSRequestService> _logger;
        private readonly IHubContext<SOSHub> _hubContext; // ✅ thêm realtime hub

        public SOSRequestService(
            IUnitOfWork unitOfWork,
            ISOSRequestOrchestrator orchestrator,
            ISOSRequestResponseMapper mapper,
            IPaginationMapper paginationMapper,
            ILogger<SOSRequestService> logger,
            IHubContext<SOSHub> hubContext) // ✅ inject hub
        {
            _unitOfWork = unitOfWork;
            _orchestrator = orchestrator;
            _mapper = mapper;
            _paginationMapper = paginationMapper;
            _logger = logger;
            _hubContext = hubContext;
        }

        // 🟢 Lấy tất cả SOSRequests
        public async Task<ResultListDTO<SOSRequestResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetAllAsync SOSRequest với page={Page}, limit={Limit}", page, limit);

            var (entities, total, last) = await _unitOfWork.SOSRequests.GetAllAsync(page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không có SOSRequest nào.");
                throw new SOSRequestNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);
            return ResultListDTO<SOSRequestResponseDTO>.Success(dtoList, "Lấy danh sách SOSRequest thành công", pagination);
        }

        // 🔵 Lấy chi tiết theo Id
        public async Task<ResultDTO<SOSRequestResponseDTO>> GetDetailAsync(Guid sosId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetDetailAsync SOS Id={Id}", sosId);

            var entity = await _unitOfWork.SOSRequests.GetByIdAsync(sosId, cancellationToken);
            if (entity is null)
            {
                _logger.LogWarning("Service: Không tìm thấy SOSRequest Id={Id}", sosId);
                throw new SOSRequestNotFoundException(sosId);
            }

            var dto = _mapper.ToDto(entity);
            return ResultDTO<SOSRequestResponseDTO>.Success(dto, "Lấy chi tiết SOSRequest thành công");
        }

        // 🟡 Lấy danh sách theo ChildId
        public async Task<ResultListDTO<SOSRequestResponseDTO>> GetByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetByChildAsync SOS cho ChildId={ChildId}", childId);

            var (entities, total, last) = await _unitOfWork.SOSRequests.GetByChildPagedAsync(childId, page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không có SOSRequest nào cho ChildId={ChildId}", childId);
                throw new SOSRequestNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);
            return ResultListDTO<SOSRequestResponseDTO>.Success(dtoList, "Lấy danh sách SOS theo trẻ thành công", pagination);
        }

        // 🆘 Tạo mới (Realtime)
        public async Task<ResultDTO<SOSRequestResponseDTO>> CreateAsync(SOSRequestCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu tạo SOSRequest cho ChildId={ChildId}", dto.ChildId);

            // 🔎 Kiểm tra trùng SOS đang xử lý
            var existingRequests = await _unitOfWork.SOSRequests.GetByChildIdAsync(dto.ChildId, cancellationToken);
            if (existingRequests.Any(r => r.TrangThai == "Đang xử lý"))
                throw new DuplicateSOSRequestException(dto.ChildId, dto.ThoiGian);

            // 🏗️ Tạo SOSRequest mới thông qua orchestrator
            var created = await _orchestrator.CreateAsync(dto, cancellationToken);

            // 🧩 Map sang ResponseDTO
            var response = new SOSRequestResponseDTO
            {
                SOSId = created.SOSId,
                ChildId = created.ChildId,
                ThoiGian = created.ThoiGian,
                ViDo = created.ViDo,
                KinhDo = created.KinhDo,
                TrangThai = created.TrangThai
            };

            // 🚨 Gửi realtime tới đúng group child
            await _hubContext.Clients.Group(dto.ChildId.ToString()).SendAsync("ReceiveSOS", new
            {
                sosId = response.SOSId,
                childId = response.ChildId,
                thoiGian = response.ThoiGian,
                viDo = response.ViDo,
                kinhDo = response.KinhDo,
                trangThai = response.TrangThai,
                message = "🚨 SOS mới được gửi!",
                time = DateTime.Now
            }, cancellationToken);

            _logger.LogInformation("Service: Tạo SOSRequest thành công và đã gửi realtime tới group {GroupId}", dto.ChildId);

            return ResultDTO<SOSRequestResponseDTO>.Success(response, "Tạo yêu cầu SOS thành công");
        }

        // 🟠 Cập nhật (Realtime)
        public async Task<ResultDTO<SOSRequestResponseDTO>> UpdateAsync(SOSRequestUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var updated = await _orchestrator.UpdateAsync(dto, cancellationToken);
            var response = _mapper.ToDto(updated);

            // 🚨 Gửi realtime tới đúng group child
            await _hubContext.Clients.Group(updated.ChildId.ToString()).SendAsync("SOSUpdated", new
            {
                sosId = updated.SOSId,
                childId = updated.ChildId,
                trangThai = updated.TrangThai,
                message = "✅ Cập nhật trạng thái SOS thành công",
                time = DateTime.Now
            }, cancellationToken);

            return ResultDTO<SOSRequestResponseDTO>.Success(response, "Cập nhật yêu cầu SOS thành công");
        }

        // 🔴 Xóa (Realtime)
        public async Task<ResultDTO> DeleteAsync(SOSRequestDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            await _orchestrator.DeleteAsync(dto, cancellationToken);

            await _hubContext.Clients.All.SendAsync("SOSDeleted", new
            {
                SOSId = dto.SOSId,
                Message = "🗑️ SOSRequest đã bị xóa",
                Time = DateTime.Now
            }, cancellationToken);

            _logger.LogInformation("Service: Đã xóa SOSRequest Id={Id} và phát tín hiệu realtime", dto.SOSId);
            return ResultDTO.Success("Xóa yêu cầu SOS thành công");
        }
    }
}
