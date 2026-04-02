using Backend_QLTE.ChildService.Application.DTOs.Client.SOSRequest;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend_QLTE.ChildService.Application.Orchestrators
{
    public class SOSRequestOrchestrator : ISOSRequestOrchestrator
    {
        private readonly ISOSRequestFactory _factory;
        private readonly ISOSRequestDomainService _domainService;
        private readonly ISOSRequestRepository _repository;
        private readonly ILogger<SOSRequestOrchestrator> _logger;

        public SOSRequestOrchestrator(
            ISOSRequestFactory factory,
            ISOSRequestDomainService domainService,
            ISOSRequestRepository repository,
            ILogger<SOSRequestOrchestrator> logger)
        {
            _factory = factory;
            _domainService = domainService;
            _repository = repository;
            _logger = logger;
        }

        // 🟢 Tạo SOSRequest mới
        public async Task<SOSRequest> CreateAsync(SOSRequestCreateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu tạo SOSRequest cho ChildId = {ChildId}", dto.ChildId);

            // 1️⃣ Dùng Factory tạo entity từ DTO
            var entity = _factory.Create(dto);

            // 2️⃣ Lấy các SOSRequest hiện có của cùng Child
            var existingRequests = await _repository.GetByChildIdAsync(dto.ChildId, cancellationToken);

            // 3️⃣ DomainService kiểm tra hợp lệ & không bị trùng
            _domainService.EnsureCanCreate(entity, existingRequests);

            // 4️⃣ Ghi vào DB
            var created = await _repository.CreateAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: SOSRequest {Id} đã được tạo thành công cho ChildId = {ChildId}", created.SOSId, dto.ChildId);

            return created;
        }

        // 🟡 Cập nhật SOSRequest
        public async Task<SOSRequest> UpdateAsync(SOSRequestUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu cập nhật SOSRequest Id = {Id}", dto.SOSId);

            var existing = await _repository.GetByIdAsync(dto.SOSId, cancellationToken);
            if (existing == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy SOSRequest Id = {Id}", dto.SOSId);
                throw new KeyNotFoundException($"Không tìm thấy SOSRequest với Id = {dto.SOSId}");
            }

            // 🧠 Chỉ update trên entity đang được tracking
            existing.TrangThai = dto.TrangThai;
            existing.ThoiGian = DateTime.UtcNow; // hoặc giữ nguyên nếu không cần
                                                 // nếu có thể cập nhật thêm ViDo, KinhDo thì thêm ở đây

            _domainService.EnsureCanUpdate(existing);

            await _repository.UpdateAsync(existing, cancellationToken);

            _logger.LogInformation("Orchestrator: SOSRequest Id = {Id} đã được cập nhật thành công", dto.SOSId);
            return existing;
        }

        // 🔴 Xóa SOSRequest
        public async Task DeleteAsync(SOSRequestDeleteDTO dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Orchestrator: Bắt đầu xóa SOSRequest Id = {Id}", dto.SOSId);

            var entity = await _repository.GetByIdAsync(dto.SOSId, cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Orchestrator: Không tìm thấy SOSRequest Id = {Id}", dto.SOSId);
                throw new KeyNotFoundException($"Không tìm thấy SOSRequest với Id = {dto.SOSId}");
            }

            _domainService.EnsureCanDelete(entity);

            await _repository.DeleteAsync(entity, cancellationToken);

            _logger.LogInformation("Orchestrator: SOSRequest Id = {Id} đã được xóa thành công", dto.SOSId);
        }
    }
}
