using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Infrastructure.Repositories
{
    public class DeviceInfoRepository : IDeviceInfoRepository
    {
        private readonly ChildDbContext _context;

        public DeviceInfoRepository(ChildDbContext context)
        {
            _context = context;
        }

        // 🟢 Lấy danh sách thiết bị có phân trang
        public async Task<(List<DeviceInfo> devices, int total, int last)> GetAllAsync(int page, int limit, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<DeviceInfo>().AsQueryable();

            var total = await query.CountAsync(cancellationToken);
            var last = (int)Math.Ceiling((double)total / limit);

            var devices = await query
                .OrderByDescending(d => d.LanCapNhatCuoi)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return (devices, total, last);
        }

        // 🔍 Lấy thiết bị theo Id
        public async Task<DeviceInfo?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceInfos
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
        }

        // 🔍 Lấy thiết bị theo ChildId (1-1)
        public async Task<DeviceInfo?> GetByChildIdAsync(Guid childId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceInfos
                .FirstOrDefaultAsync(d => d.ChildId == childId, cancellationToken);
        }

        // 🔍 Lấy thiết bị theo IMEI
        public async Task<DeviceInfo?> GetByIMEIAsync(string imei, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceInfos
                .FirstOrDefaultAsync(d => d.IMEI == imei, cancellationToken);
        }

        // ✅ Kiểm tra IMEI đã tồn tại chưa
        public async Task<bool> ExistsByIMEIAsync(string imei, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceInfos
                .AnyAsync(d => d.IMEI == imei, cancellationToken);
        }

        // 🟢 Tạo mới
        public async Task<DeviceInfo> CreateAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default)
        {
            _context.DeviceInfos.Add(deviceInfo);
            await _context.SaveChangesAsync(cancellationToken);
            return deviceInfo;
        }

        // 🟡 Cập nhật
        public async Task<DeviceInfo> UpdateAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default)
        {
            _context.DeviceInfos.Update(deviceInfo);
            await _context.SaveChangesAsync(cancellationToken);
            return deviceInfo;
        }

        // 🔴 Xóa
        public async Task DeleteAsync(DeviceInfo deviceInfo, CancellationToken cancellationToken = default)
        {
            _context.DeviceInfos.Remove(deviceInfo);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // 🟢 Lấy tất cả thiết bị đang được theo dõi (IsTracking = true)
        public async Task<List<DeviceInfo>> GetTrackingDevicesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DeviceInfos
                .Where(d => d.IsTracking)
                .OrderByDescending(d => d.LanCapNhatCuoi)
                .ToListAsync(cancellationToken);
        }

        // 🟡 Cập nhật trạng thái khóa/mở khóa thiết bị
        public async Task<bool> SetLockStateAsync(Guid childId, bool isLocked, CancellationToken cancellationToken = default)
        {
            var device = await _context.DeviceInfos.FirstOrDefaultAsync(d => d.ChildId == childId, cancellationToken);
            if (device == null) return false;

            device.IsLocked = isLocked;
            device.LanCapNhatCuoi = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
