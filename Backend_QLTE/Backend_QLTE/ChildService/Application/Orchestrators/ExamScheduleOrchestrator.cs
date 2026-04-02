using Backend_QLTE.ChildService.Application.DTOs.Client.ExamSchedule;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Orchestrators
{
    public class ExamScheduleOrchestrator : IExamScheduleOrchestrator
    {
        private readonly IExamScheduleFactory _factory;
        private readonly IExamScheduleDomainService _domainService;
        private readonly IExamScheduleRepository _repository;
        private readonly ILogger<ExamScheduleOrchestrator> _logger;

        public ExamScheduleOrchestrator(
            IExamScheduleFactory factory,
            IExamScheduleDomainService domainService,
            IExamScheduleRepository repository,
            ILogger<ExamScheduleOrchestrator> logger)
        {
            _factory = factory;
            _domainService = domainService;
            _repository = repository;
            _logger = logger;
        }

        // 🟢 Tạo mới ExamSchedule
        public async Task<ExamSchedule> CreateAsync(ExamScheduleCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu tạo ExamSchedule cho ChildId = {ChildId}", dto.ChildId);

            var entity = _factory.Create(dto);

            // 🟢 Lấy danh sách lịch thi hiện có của cùng 1 trẻ
            var existingExams = await _repository.GetByChildIdAsync(dto.ChildId, cancellationToken);

            // 🟠 DomainService: kiểm tra hợp lệ và trùng giờ thi
            _domainService.EnsureCanCreate(entity, existingExams);

            var created = await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: ExamSchedule {Id} đã được tạo thành công cho ChildId = {ChildId}", created.ExamId, dto.ChildId);

            return created;
        }

        // 🟡 Cập nhật ExamSchedule
        public async Task<ExamSchedule> UpdateAsync(ExamScheduleUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu cập nhật ExamSchedule Id = {Id}", dto.ExamId);

            var existing = await _repository.GetByIdAsync(dto.ExamId, cancellationToken);
            if (existing == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy ExamSchedule Id = {Id}", dto.ExamId);
                throw new KeyNotFoundException($"Không tìm thấy ExamSchedule với Id = {dto.ExamId}");
            }

            var updatedEntity = _factory.Update(dto);

            _domainService.EnsureCanUpdate(updatedEntity);

            var updated = await _repository.UpdateAsync(updatedEntity, cancellationToken);

            _logger.LogInformation("Orchestrator: ExamSchedule Id = {Id} đã được cập nhật thành công", dto.ExamId);

            return updated;
        }

        // 🔴 Xóa ExamSchedule
        public async Task DeleteAsync(ExamScheduleDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu xóa ExamSchedule Id = {Id}", dto.ExamId);

            var entity = await _repository.GetByIdAsync(dto.ExamId, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy ExamSchedule Id = {Id}", dto.ExamId);
                throw new KeyNotFoundException($"Không tìm thấy ExamSchedule với Id = {dto.ExamId}");
            }

            _domainService.EnsureCanDelete(entity);

            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: ExamSchedule Id = {Id} đã được xóa thành công", dto.ExamId);
        }
    }
}
