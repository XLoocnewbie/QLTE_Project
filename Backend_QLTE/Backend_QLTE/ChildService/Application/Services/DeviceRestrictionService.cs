using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.DTOs.Client.DeviceRestriction;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Backend_QLTE.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class DeviceRestrictionService : IDeviceRestrictionService
    {
        private readonly ChildDbContext _context;
        private readonly ILogger<DeviceRestrictionService> _logger;
        private readonly IHubContext<RestrictionHub> _hubContext;

        public DeviceRestrictionService(
            ChildDbContext context,
            ILogger<DeviceRestrictionService> logger,
            IHubContext<RestrictionHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        // 🟢 Lấy danh sách hạn chế theo DeviceId
        public async Task<ResultListDTO<DeviceRestrictionResponseDTO>> GetByDeviceIdAsync(Guid deviceId)
        {
            if (deviceId == Guid.Empty)
                throw new ApiException("DeviceId không hợp lệ!", 400);

            var list = await _context.DeviceRestrictions
                .Where(r => r.DeviceId == deviceId)
                .OrderByDescending(r => r.UpdatedAt)
                .ToListAsync();

            if (!list.Any())
                throw new ApiException($"Không tìm thấy cấu hình hạn chế cho DeviceId={deviceId}", 404);

            var dtoList = list.Select(r => new DeviceRestrictionResponseDTO
            {
                RestrictionId = r.RestrictionId,
                DeviceId = r.DeviceId,
                BlockedApps = r.BlockedApps,
                BlockedDomains = r.BlockedDomains,
                AllowedDomains = r.AllowedDomains,
                IsFirewallEnabled = r.IsFirewallEnabled,
                Mode = r.Mode,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return ResultListDTO<DeviceRestrictionResponseDTO>.Success(dtoList, "Lấy danh sách hạn chế thành công");
        }

        // 🔵 Lấy chi tiết cấu hình
        public async Task<ResultDTO<DeviceRestrictionResponseDTO>> GetDetailAsync(Guid restrictionId)
        {
            var entity = await _context.DeviceRestrictions
                .FirstOrDefaultAsync(r => r.RestrictionId == restrictionId);

            if (entity == null)
                throw new ApiException($"Không tìm thấy RestrictionId={restrictionId}", 404);

            var dto = new DeviceRestrictionResponseDTO
            {
                RestrictionId = entity.RestrictionId,
                DeviceId = entity.DeviceId,
                BlockedApps = entity.BlockedApps,
                BlockedDomains = entity.BlockedDomains,
                AllowedDomains = entity.AllowedDomains,
                IsFirewallEnabled = entity.IsFirewallEnabled,
                Mode = entity.Mode,
                UpdatedAt = entity.UpdatedAt
            };

            return ResultDTO<DeviceRestrictionResponseDTO>.Success(dto, "Lấy chi tiết cấu hình thành công");
        }

        // 🟡 Tạo mới cấu hình hạn chế
        public async Task<ResultDTO<DeviceRestrictionResponseDTO>> CreateAsync(DeviceRestrictionCreateDTO dto)
        {
            if (dto.DeviceId == Guid.Empty)
                throw new ApiException("DeviceId không được để trống", 400);

            var device = await _context.DeviceInfos.FirstOrDefaultAsync(d => d.DeviceId == dto.DeviceId);
            if (device == null)
                throw new ApiException($"Không tồn tại DeviceId={dto.DeviceId}", 404);

            var entity = new DeviceRestriction
            {
                RestrictionId = Guid.NewGuid(),
                DeviceId = dto.DeviceId,
                BlockedApps = dto.BlockedApps,
                BlockedDomains = dto.BlockedDomains,
                AllowedDomains = dto.AllowedDomains,
                IsFirewallEnabled = dto.IsFirewallEnabled,
                Mode = dto.Mode,
                UpdatedAt = DateTime.Now
            };

            await _context.DeviceRestrictions.AddAsync(entity);
            await _context.SaveChangesAsync();

            var response = new DeviceRestrictionResponseDTO
            {
                RestrictionId = entity.RestrictionId,
                DeviceId = entity.DeviceId,
                BlockedApps = entity.BlockedApps,
                BlockedDomains = entity.BlockedDomains,
                AllowedDomains = entity.AllowedDomains,
                IsFirewallEnabled = entity.IsFirewallEnabled,
                Mode = entity.Mode,
                UpdatedAt = entity.UpdatedAt
            };

            _logger.LogInformation("Tạo DeviceRestriction cho DeviceId={DeviceId} thành công", dto.DeviceId);

            // 📡 Gửi realtime cho child app
            await _hubContext.Clients.Group($"device-{entity.DeviceId}")
                .SendAsync("OnRestrictionChanged", new
                {
                    action = "created",
                    restriction = response
                });

            return ResultDTO<DeviceRestrictionResponseDTO>.Success(response, "Tạo cấu hình hạn chế thành công");
        }

        // 🟠 Cập nhật cấu hình hạn chế
        public async Task<ResultDTO<DeviceRestrictionUpdateResponseDTO>> UpdateAsync(DeviceRestrictionUpdateDTO dto)
        {
            var entity = await _context.DeviceRestrictions
                .FirstOrDefaultAsync(r => r.RestrictionId == dto.RestrictionId);

            if (entity == null)
                throw new ApiException($"Không tìm thấy RestrictionId={dto.RestrictionId}", 404);

            entity.BlockedApps = dto.BlockedApps;
            entity.BlockedDomains = dto.BlockedDomains;
            entity.AllowedDomains = dto.AllowedDomains;
            entity.IsFirewallEnabled = dto.IsFirewallEnabled;
            entity.Mode = dto.Mode;
            entity.UpdatedAt = DateTime.Now;

            _context.DeviceRestrictions.Update(entity);
            await _context.SaveChangesAsync();

            var response = new DeviceRestrictionUpdateResponseDTO
            {
                RestrictionId = entity.RestrictionId,
                DeviceId = entity.DeviceId,
                BlockedApps = entity.BlockedApps,
                BlockedDomains = entity.BlockedDomains,
                AllowedDomains = entity.AllowedDomains,
                IsFirewallEnabled = entity.IsFirewallEnabled,
                Mode = entity.Mode,
                UpdatedAt = entity.UpdatedAt
            };

            _logger.LogInformation("Cập nhật DeviceRestriction Id={Id} thành công", dto.RestrictionId);

            // 📡 Gửi realtime event
            await _hubContext.Clients.Group($"device-{entity.DeviceId}")
                .SendAsync("OnRestrictionChanged", new
                {
                    action = "updated",
                    restriction = response
                });

            return ResultDTO<DeviceRestrictionUpdateResponseDTO>.Success(response, "Cập nhật cấu hình hạn chế thành công");
        }

        // 🔴 Xóa cấu hình hạn chế
        public async Task<ResultDTO> DeleteAsync(DeviceRestrictionDeleteDTO dto)
        {
            var entity = await _context.DeviceRestrictions
                .FirstOrDefaultAsync(r => r.RestrictionId == dto.RestrictionId);

            if (entity == null)
                throw new ApiException($"Không tìm thấy RestrictionId={dto.RestrictionId}", 404);

            _context.DeviceRestrictions.Remove(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa DeviceRestriction Id={Id} thành công", dto.RestrictionId);

            // 📡 Gửi realtime thông báo xóa
            await _hubContext.Clients.Group($"device-{entity.DeviceId}")
                .SendAsync("OnRestrictionChanged", new
                {
                    action = "deleted",
                    restrictionId = dto.RestrictionId
                });

            return ResultDTO.Success("Xóa cấu hình hạn chế thành công");
        }

        // 🟣 Bật / tắt Firewall realtime
        public async Task<ResultDTO<DeviceRestrictionUpdateResponseDTO>> ToggleFirewallAsync(Guid restrictionId)
        {
            var entity = await _context.DeviceRestrictions
                .FirstOrDefaultAsync(r => r.RestrictionId == restrictionId);

            if (entity == null)
                throw new ApiException($"Không tìm thấy RestrictionId={restrictionId}", 404);

            entity.IsFirewallEnabled = !entity.IsFirewallEnabled;
            entity.UpdatedAt = DateTime.Now;

            _context.DeviceRestrictions.Update(entity);
            await _context.SaveChangesAsync();

            var msg = entity.IsFirewallEnabled ? "Đã bật Firewall" : "Đã tắt Firewall";
            _logger.LogInformation("{Msg} cho DeviceRestriction Id={Id}", msg, restrictionId);

            var response = new DeviceRestrictionUpdateResponseDTO
            {
                RestrictionId = entity.RestrictionId,
                DeviceId = entity.DeviceId,
                BlockedApps = entity.BlockedApps,
                BlockedDomains = entity.BlockedDomains,
                AllowedDomains = entity.AllowedDomains,
                IsFirewallEnabled = entity.IsFirewallEnabled,
                Mode = entity.Mode,
                UpdatedAt = entity.UpdatedAt
            };

            // 📡 Gửi realtime về device group
            await _hubContext.Clients.Group($"device-{entity.DeviceId}")
                .SendAsync("OnFirewallToggled", new
                {
                    deviceId = entity.DeviceId,
                    isEnabled = entity.IsFirewallEnabled,
                    message = msg,
                    updatedAt = entity.UpdatedAt
                });

            return ResultDTO<DeviceRestrictionUpdateResponseDTO>.Success(response, msg);
        }

        // ✅ Bật chế độ StudyMode restriction (khi bắt đầu giờ học)
        public async Task<ResultDTO> ActivateStudyRestrictionAsync(Guid deviceId)
        {
            var entity = await _context.DeviceRestrictions
                .FirstOrDefaultAsync(r => r.DeviceId == deviceId && r.Mode == "StudyMode");

            if (entity == null)
            {
                // Tạo mới nếu chưa có
                entity = new DeviceRestriction
                {
                    RestrictionId = Guid.NewGuid(),
                    DeviceId = deviceId,
                    BlockedApps = "youtube,facebook,tiktok,chrome",
                    BlockedDomains = "facebook.com,youtube.com,tiktok.com",
                    AllowedDomains = "dictionary.com,googleclassroom.com",
                    IsFirewallEnabled = true,
                    Mode = "StudyMode",
                    UpdatedAt = DateTime.Now
                };

                await _context.DeviceRestrictions.AddAsync(entity);
            }
            else
            {
                // Nếu đã có thì chỉ bật lại firewall
                entity.IsFirewallEnabled = true;
                entity.UpdatedAt = DateTime.Now;
                _context.DeviceRestrictions.Update(entity);
            }

            await _context.SaveChangesAsync();

            // 📡 Gửi realtime cho child app
            await _hubContext.Clients.Group($"device-{deviceId}")
                .SendAsync("OnRestrictionChanged", new
                {
                    action = "activated",
                    restriction = new
                    {
                        entity.RestrictionId,
                        entity.DeviceId,
                        entity.BlockedApps,
                        entity.BlockedDomains,
                        entity.AllowedDomains,
                        entity.IsFirewallEnabled,
                        entity.Mode,
                        entity.UpdatedAt
                    }
                });

            _logger.LogInformation("Đã bật Restriction StudyMode cho device-{DeviceId}", deviceId);
            return ResultDTO.Success("Đã bật Restriction StudyMode thành công");
        }

        // 🚫 Tắt Restriction (khi kết thúc giờ học)
        public async Task<ResultDTO> DeactivateRestrictionAsync(Guid deviceId)
        {
            var list = await _context.DeviceRestrictions
                .Where(r => r.DeviceId == deviceId && r.IsFirewallEnabled)
                .ToListAsync();

            if (!list.Any())
                return ResultDTO.Success("Không có Restriction nào đang bật.");

            foreach (var r in list)
            {
                r.IsFirewallEnabled = false;
                r.UpdatedAt = DateTime.Now;
                _context.DeviceRestrictions.Update(r);
            }

            await _context.SaveChangesAsync();

            // 📡 Gửi realtime cho child app
            await _hubContext.Clients.Group($"device-{deviceId}")
                .SendAsync("OnRestrictionChanged", new
                {
                    action = "deactivated",
                    deviceId,
                    message = "Tất cả Restriction đã được tắt."
                });

            _logger.LogInformation("Đã tắt Restriction cho device-{DeviceId}", deviceId);
            return ResultDTO.Success("Đã tắt Restriction thành công");
        }
    }
}
