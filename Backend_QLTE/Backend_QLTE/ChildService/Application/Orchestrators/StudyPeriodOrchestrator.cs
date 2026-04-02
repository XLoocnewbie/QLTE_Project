using Backend_QLTE.ChildService.Application.DTOs.Client.StudyPeriod;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Orchestrators
{
    public class StudyPeriodOrchestrator : IStudyPeriodOrchestrator
    {
        private readonly IStudyPeriodFactory _factory;
        private readonly IStudyPeriodDomainService _domainService;
        private readonly IStudyPeriodRepository _repository;
        private readonly ILogger<StudyPeriodOrchestrator> _logger;

        public StudyPeriodOrchestrator(
            IStudyPeriodFactory factory,
            IStudyPeriodDomainService domainService,
            IStudyPeriodRepository repository,
            ILogger<StudyPeriodOrchestrator> logger)
        {
            _factory = factory;
            _domainService = domainService;
            _repository = repository;
            _logger = logger;
        }

        // 🟢 Tạo StudyPeriod mới
        public async Task<StudyPeriod> CreateAsync(StudyPeriodCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu tạo StudyPeriod cho ChildId = {ChildId}", dto.ChildId);

            var entity = _factory.Create(dto);

            _domainService.EnsureCanCreate(entity);

            var created = await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: StudyPeriod {Id} đã được tạo thành công cho ChildId = {ChildId}", created.StudyPeriodId, dto.ChildId);

            return created;
        }

        // 🟡 Cập nhật StudyPeriod
        public async Task<StudyPeriod> UpdateAsync(StudyPeriodUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu cập nhật StudyPeriod Id = {Id}", dto.StudyPeriodId);

            var existing = await _repository.GetByIdAsync(dto.StudyPeriodId, cancellationToken);
            if (existing == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy StudyPeriod Id = {Id}", dto.StudyPeriodId);
                throw new KeyNotFoundException($"Không tìm thấy StudyPeriod với Id = {dto.StudyPeriodId}");
            }

            var updatedEntity = _factory.Update(dto);

            _domainService.EnsureCanUpdate(updatedEntity);

            var updated = await _repository.UpdateAsync(updatedEntity, cancellationToken);

            _logger.LogInformation("Orchestrator: StudyPeriod Id = {Id} đã được cập nhật thành công", dto.StudyPeriodId);

            return updated;
        }

        // 🔴 Xóa StudyPeriod
        public async Task DeleteAsync(StudyPeriodDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu xóa StudyPeriod Id = {Id}", dto.StudyPeriodId);

            var entity = await _repository.GetByIdAsync(dto.StudyPeriodId, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy StudyPeriod Id = {Id}", dto.StudyPeriodId);
                throw new KeyNotFoundException($"Không tìm thấy StudyPeriod với Id = {dto.StudyPeriodId}");
            }

            _domainService.EnsureCanDelete(entity);

            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: StudyPeriod Id = {Id} đã được xóa thành công", dto.StudyPeriodId);
        }
    }
}
