using Backend_QLTE.ChildService.Application.DTOs.Client.Schedule;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Orchestrators
{
    public class ScheduleOrchestrator : IScheduleOrchestrator
    {
        private readonly IScheduleFactory _factory;
        private readonly IScheduleDomainService _domainService;
        private readonly IScheduleRepository _repository;
        private readonly ILogger<ScheduleOrchestrator> _logger;

        public ScheduleOrchestrator(
            IScheduleFactory factory,
            IScheduleDomainService domainService,
            IScheduleRepository repository,
            ILogger<ScheduleOrchestrator> logger)
        {
            _factory = factory;
            _domainService = domainService;
            _repository = repository;
            _logger = logger;
        }

        // 🟢 Tạo mới Schedule
        public async Task<Schedule> CreateAsync(ScheduleCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu tạo Schedule cho ChildId = {ChildId}", dto.ChildId);

            var entity = _factory.Create(dto);

            // 🟢 Lấy danh sách lịch hiện có của cùng 1 trẻ để kiểm tra trùng giờ
            var existingSchedules = await _repository.GetByChildIdAsync(dto.ChildId, cancellationToken);

            // 🟠 DomainService: kiểm tra hợp lệ và trùng giờ
            _domainService.EnsureCanCreate(entity, existingSchedules);

            var created = await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: Schedule {Id} đã được tạo thành công cho ChildId = {ChildId}", created.ScheduleId, dto.ChildId);

            return created;
        }

        // 🟡 Cập nhật Schedule
        public async Task<Schedule> UpdateAsync(ScheduleUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu cập nhật Schedule Id = {Id}", dto.ScheduleId);

            // 🔹 1. Lấy entity hiện có trong DB
            var existing = await _repository.GetByIdAsync(dto.ScheduleId, cancellationToken);
            if (existing == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy Schedule Id = {Id}", dto.ScheduleId);
                throw new KeyNotFoundException($"Không tìm thấy Schedule với Id = {dto.ScheduleId}");
            }

            // 🔹 2. Giữ nguyên ChildId gốc (rất quan trọng)
            dto.ChildId = existing.ChildId;

            // 🔹 3. Gọi factory build lại entity
            var updatedEntity = _factory.Update(dto);

            // 🔹 4. Domain validation
            _domainService.EnsureCanUpdate(updatedEntity);

            // 🔹 5. Update DB
            var updated = await _repository.UpdateAsync(updatedEntity, cancellationToken);

            _logger.LogInformation("Orchestrator: Schedule Id = {Id} đã được cập nhật thành công", dto.ScheduleId);

            return updated;
        }

        // 🔴 Xóa Schedule
        public async Task DeleteAsync(ScheduleDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu xóa Schedule Id = {Id}", dto.ScheduleId);

            var entity = await _repository.GetByIdAsync(dto.ScheduleId, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy Schedule Id = {Id}", dto.ScheduleId);
                throw new KeyNotFoundException($"Không tìm thấy Schedule với Id = {dto.ScheduleId}");
            }

            _domainService.EnsureCanDelete(entity);

            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: Schedule Id = {Id} đã được xóa thành công", dto.ScheduleId);
        }
    }
}
