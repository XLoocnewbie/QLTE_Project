using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Exceptions.Duplicates;
using Backend_QLTE.ChildService.Application.Exceptions.Failed;
using Backend_QLTE.ChildService.Application.Exceptions.NotFounds;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExamScheduleOrchestrator _orchestrator;
        private readonly IExamScheduleResponseMapper _mapper;
        private readonly IPaginationMapper _paginationMapper;
        private readonly ILogger<ExamScheduleService> _logger;

        public ExamScheduleService(
            IUnitOfWork unitOfWork,
            IExamScheduleOrchestrator orchestrator,
            IExamScheduleResponseMapper mapper,
            IPaginationMapper paginationMapper,
            ILogger<ExamScheduleService> logger)
        {
            _unitOfWork = unitOfWork;
            _orchestrator = orchestrator;
            _mapper = mapper;
            _paginationMapper = paginationMapper;
            _logger = logger;
        }

        // 🟢 Lấy danh sách tất cả ExamSchedule có phân trang
        public async Task<ResultListDTO<ExamScheduleResponseDTO>> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetAllAsync ExamSchedule page={Page}, limit={Limit}", page, limit);

            var (entities, total, last) = await _unitOfWork.ExamSchedules.GetAllAsync(page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không tìm thấy ExamSchedule nào");
                throw new ExamScheduleNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);
            return ResultListDTO<ExamScheduleResponseDTO>.Success(dtoList, "Lấy danh sách ExamSchedule thành công", pagination);
        }

        // 🔵 Lấy chi tiết 1 lịch thi
        public async Task<ResultDTO<ExamScheduleResponseDTO>> GetDetailAsync(Guid examId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetDetailAsync ExamSchedule Id={Id}", examId);

            var entity = await _unitOfWork.ExamSchedules.GetByIdAsync(examId, cancellationToken);
            if (entity is null)
            {
                _logger.LogWarning("Service: Không tìm thấy ExamSchedule Id={Id}", examId);
                throw new ExamScheduleNotFoundException(examId);
            }

            var dto = _mapper.ToDto(entity);
            return ResultDTO<ExamScheduleResponseDTO>.Success(dto, "Lấy chi tiết ExamSchedule thành công");
        }

        // 🟢 Lấy tất cả lịch thi theo trẻ
        public async Task<ResultListDTO<ExamScheduleResponseDTO>> GetAllByChildAsync(Guid childId, int page, int limit, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetAllByChildAsync ExamSchedule ChildId={ChildId}", childId);

            var (entities, total, last) = await _unitOfWork.ExamSchedules.GetByChildPagedAsync(childId, page, limit, cancellationToken);
            if (total == 0)
            {
                _logger.LogWarning("Service: Không tìm thấy ExamSchedule cho ChildId={ChildId}", childId);
                throw new ExamScheduleNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(entities);
            var pagination = _paginationMapper.ToDto(page, limit, total, last);
            return ResultListDTO<ExamScheduleResponseDTO>.Success(dtoList, "Lấy danh sách ExamSchedule theo trẻ thành công", pagination);
        }

        // 🕓 Lấy danh sách lịch thi sắp tới
        public async Task<ResultListDTO<ExamScheduleResponseDTO>> GetUpcomingExamsAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: GetUpcomingExamsAsync cho ChildId={ChildId}", childId);

            var upcoming = await _unitOfWork.ExamSchedules.GetUpcomingExamsAsync(childId, DateTime.Now, cancellationToken);
            if (!upcoming.Any())
            {
                _logger.LogWarning("Service: Không có lịch thi sắp tới cho ChildId={ChildId}", childId);
                throw new ExamScheduleNotFoundException();
            }

            var dtoList = _mapper.ToDtoList(upcoming);
            return ResultListDTO<ExamScheduleResponseDTO>.Success(dtoList, "Lấy danh sách lịch thi sắp tới thành công");
        }

        // 🟢 Create
        public async Task<ResultDTO> CreateAsync(ExamScheduleCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu CreateAsync ExamSchedule cho ChildId={ChildId}", dto.ChildId);

            var existingExams = await _unitOfWork.ExamSchedules.GetByChildIdAsync(dto.ChildId, cancellationToken);
            if (existingExams.Any(e => e.ThoiGianThi == dto.ThoiGianThi && e.MonThi == dto.MonThi))
                throw new DuplicateExamScheduleException(dto.MonThi, dto.ThoiGianThi);

            await _orchestrator.CreateAsync(dto, cancellationToken);

            _logger.LogInformation("Service: Tạo ExamSchedule cho ChildId={ChildId} thành công", dto.ChildId);
            return ResultDTO.Success("Tạo ExamSchedule thành công");
        }

        // 🟡 Update
        public async Task<ResultDTO<ExamScheduleResponseDTO>> UpdateAsync(ExamScheduleUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu UpdateAsync ExamSchedule Id={Id}", dto.ExamId);

            var updatedEntity = await _orchestrator.UpdateAsync(dto, cancellationToken);

            var response = _mapper.ToDto(updatedEntity);
            return ResultDTO<ExamScheduleResponseDTO>.Success(response, "Cập nhật ExamSchedule thành công");
        }

        // 🔴 Delete
        public async Task<ResultDTO> DeleteAsync(ExamScheduleDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Service: Bắt đầu DeleteAsync ExamSchedule Id={Id}", dto.ExamId);

            await _orchestrator.DeleteAsync(dto, cancellationToken);

            _logger.LogInformation("Service: Xoá ExamSchedule Id={Id} thành công", dto.ExamId);
            return ResultDTO.Success("Xóa ExamSchedule thành công");
        }
    }
}
