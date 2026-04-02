using Backend_QLTE.ChildService.Application.DTOs.Client.Child;
using Backend_QLTE.ChildService.Application.DTOs.Common;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Domain.Entities;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.shared.Exceptions;
using Backend_QLTE.UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend_QLTE.ChildService.Application.Services
{
    public class ZoneService : IZoneService
    {
        private readonly ChildDbContext _childDbContext;
        private readonly UserManager<User> _userManager;

        public ZoneService(ChildDbContext childDbContext, UserManager<User> userManager)
        {
            _childDbContext = childDbContext;
            _userManager = userManager;
        }

        public async Task<ResultListDTO<SafeZoneResponseDTO>> GetSafeZoneByUserIdAsync(string userId)
        {
            var get = await _childDbContext.SafeZones.Where(s => s.UserId == userId).ToListAsync();
            if (get.Count == 0)
            {
                throw new ApiException($"Không thấy danh sách vùng an toàn của UserId '{userId}' thất bại!", 404);
            }
            var result = get.Select(g => new SafeZoneResponseDTO
            {
                SafeZoneId = g.SafeZoneId,
                TenZone = g.TenZone,
                BanKinh = g.BanKinh,
                ChildrenId = g.ChildrenId,
                KinhDo = g.KinhDo,
                UserId = userId,
                ViDo = g.ViDo,
            }).ToList();
            return ResultListDTO<SafeZoneResponseDTO>.Success(result, $"Lấy danh sách vùng an toàn của userid '{userId}' thành công.");
        }

        public async Task<ResultListDTO<SafeZoneResponseDTO>> GetSafeZoneByUserIdAndChildIdAsync(string userId, Guid childId)
        {
            var get = await _childDbContext.SafeZones.Where(s => s.UserId == userId && s.ChildrenId == childId).ToListAsync();
            if (get.Count == 0)
            {
                throw new ApiException($"Không thấy danh sách vùng an toàn của UserId '{userId}' và ChildId '{childId}' thất bại!", 404);
            }
            var result = get.Select(g => new SafeZoneResponseDTO
            {
                SafeZoneId = g.SafeZoneId,
                TenZone = g.TenZone,
                BanKinh = g.BanKinh,
                ChildrenId = g.ChildrenId,
                KinhDo = g.KinhDo,
                UserId = userId,
                ViDo = g.ViDo,
            }).ToList();
            return ResultListDTO<SafeZoneResponseDTO>.Success(result, $"Lấy danh sách vùng an toàn của userid '{userId}' thành công.");
        }

        public async Task<ResultDTO> CreateSafeZoneAsync(CreateSafeZoneRequestDTO request)
        {
            var child = await _childDbContext.Children.FirstOrDefaultAsync(c => c.ChildId == request.ChildrenId);
            if (child == null)
            {
                throw new ApiException("Không tồn tại id chilren trong hệ thống", 404);
            }

            var parent = await _userManager.FindByIdAsync(request.UserId);
            if (parent == null)
            {
                throw new ApiException("Không tồn tại id user trong hệ thống", 404);
            }

            var create = new SafeZone
            {
                SafeZoneId = Guid.NewGuid(),
                TenZone = request.TenZone,
                UserId = request.UserId,
                ChildrenId = request.ChildrenId,
                KinhDo = request.KinhDo,
                ViDo = request.ViDo,
                BanKinh = request.BanKinh,
            };

            var createSafeZone = await _childDbContext.SafeZones.AddAsync(create);
            if (createSafeZone == null)
            {
                throw new ApiException("Tạo vùng an toàn thất bại");
            }
            await _childDbContext.SaveChangesAsync();

            return ResultDTO.Success("Tạo vùng an toàn thành công");
        }

        public async Task<ResultDTO<SafeZoneResponseDTO>> UpdateSafeZoneAsync(UpdateSafeZoneRequestDTO request)
        {

            var safe = await _childDbContext.SafeZones.FirstOrDefaultAsync(s => s.SafeZoneId == request.SafeZoneId);
            if (safe == null)
            {
                throw new ApiException("Không tồn tại id safe zone trong hệ thống", 404);
            }

            safe.TenZone = request.TenZone;
            safe.KinhDo = request.KinhDo;
            safe.ViDo = request.ViDo;
            safe.BanKinh = request.BanKinh;

            var createSafeZone = _childDbContext.SafeZones.Update(safe);
            if (createSafeZone == null)
            {
                throw new ApiException("Update vùng an toàn thất bại");
            }
            await _childDbContext.SaveChangesAsync();

            var result = new SafeZoneResponseDTO
            {
                SafeZoneId = request.SafeZoneId,
                UserId = safe.UserId,
                ChildrenId = safe.ChildrenId,
                TenZone = request.TenZone,
                KinhDo = request.KinhDo,
                ViDo = request.ViDo,
                BanKinh = safe.BanKinh

            };

            return ResultDTO<SafeZoneResponseDTO>.Success(result, "Update vùng an toàn thành công");
        }

        public async Task<ResultDTO> DeleteSafeZoneAsync(Guid safeZoneId)
        {
            var safeZone = await _childDbContext.SafeZones.FirstOrDefaultAsync(s => s.SafeZoneId == safeZoneId);
            if (safeZone == null)
            {
                throw new ApiException($"Không tồn tại safeZoneId '{safeZoneId}' trong hệ thống!", 404);
            }

            var delete = _childDbContext.SafeZones.Remove(safeZone);
            if (delete == null)
            {
                throw new ApiException($"Xóa safeZoneId '{safeZoneId}' thất bại");
            }
            await _childDbContext.SaveChangesAsync();
            return ResultDTO.Success($"Xóa vùng an toàn với safeZoneId '{safeZoneId}' thành công.");
        }

        public async Task<ResultDTO> DeleteSafeZoneByChildrenIdAsync(Guid childrenId)
        {
            var safeZones = await _childDbContext.SafeZones.Where(s => s.ChildrenId == childrenId).ToListAsync();
            if (safeZones == null || safeZones.Count == 0)
            {
                throw new ApiException($"Không tồn tại vùng an toàn cho ChildrenId '{childrenId}' trong hệ thống!", 404);
            }

            _childDbContext.SafeZones.RemoveRange(safeZones);
            await _childDbContext.SaveChangesAsync();
            return ResultDTO.Success($"Xóa vùng an toàn với childrenId '{childrenId}' thành công.");
        }

        public async Task<ResultListDTO<DangerZoneResponseDTO>> GetDangerZoneByUserIdAsync(string userId)
        {
            var get = await _childDbContext.DangerZones.Where(s => s.UserId == userId).ToListAsync();
            if (get.Count == 0)
            {
                throw new ApiException($"Không thấy danh sách vùng an toàn của UserId '{userId}' thất bại!", 404);
            }
            var result = get.Select(g => new DangerZoneResponseDTO
            {
                DangerZoneId = g.DangerZoneId,
                TenKhuVuc = g.TenKhuVuc,
                BanKinh = g.BanKinh,
                ChildrenId = g.ChildrenId,
                KinhDo = g.KinhDo,
                UserId = userId,
                ViDo = g.ViDo,
                MoTa = g.MoTa,
            }).ToList();
            return ResultListDTO<DangerZoneResponseDTO>.Success(result, $"Lấy danh sách vùng an toàn của userid '{userId}' thành công.");
        }

        public async Task<ResultListDTO<DangerZoneResponseDTO>> GetDangerZoneByUserIdAndChildIdAsync(string userId, Guid childId)
        {
            var get = await _childDbContext.DangerZones.Where(s => s.UserId == userId && s.ChildrenId == childId).ToListAsync();
            if (get.Count == 0)
            {
                throw new ApiException($"Không thấy danh sách vùng an toàn của UserId '{userId}' và ChildId '{childId}' thất bại!", 404);
            }
            var result = get.Select(g => new DangerZoneResponseDTO
            {
                DangerZoneId = g.DangerZoneId,
                TenKhuVuc = g.TenKhuVuc,
                BanKinh = g.BanKinh,
                ChildrenId = g.ChildrenId,
                KinhDo = g.KinhDo,
                UserId = userId,
                ViDo = g.ViDo,
                MoTa = g.MoTa,
            }).ToList();
            return ResultListDTO<DangerZoneResponseDTO>.Success(result, $"Lấy danh sách vùng an toàn của userid '{userId}' và ChildId '{childId} thành công.");
        }

        public async Task<ResultDTO> CreateDangerZoneAsync(CreateDangerZoneRequestDTO request)
        {
            Guid childrenId = Guid.Parse(request.ChildrenId);
            var child = await _childDbContext.Children.FirstOrDefaultAsync(c => c.ChildId == childrenId);
            if (child == null)
            {
                throw new ApiException("Không tồn tại id chilren trong hệ thống", 404);
            }

            var parent = await _userManager.FindByIdAsync(request.UserId);
            if (parent == null)
            {
                throw new ApiException("Không tồn tại id user trong hệ thống", 404);
            }

            var create = new DangerZone
            {
                DangerZoneId = Guid.NewGuid(),
                TenKhuVuc = request.TenKhuVuc,
                MoTa = request.Mota,
                UserId = request.UserId,
                ChildrenId = childrenId,
                KinhDo = request.KinhDo,
                ViDo = request.ViDo,
                BanKinh = request.BanKinh,
            };

            var createDangerZone = await _childDbContext.DangerZones.AddAsync(create);
            if (createDangerZone == null)
            {
                throw new ApiException("Tạo vùng nguy hiểm thất bại");
            }
            await _childDbContext.SaveChangesAsync();

            return ResultDTO.Success("Tạo vùng nguy hiểm thành công");
        }

        public async Task<ResultDTO<DangerZoneResponseDTO>> UpdateDangerZoneAsync(UpdateDangerZoneRequestDTO request)
        {

            var safe = await _childDbContext.DangerZones.FirstOrDefaultAsync(s => s.DangerZoneId == request.DangerZoneId);
            if (safe == null)
            {
                throw new ApiException("Không tồn tại id danger zone trong hệ thống", 404);
            }

            safe.TenKhuVuc = request.TenKhuVuc;
            safe.KinhDo = request.KinhDo;
            safe.ViDo = request.ViDo;
            safe.BanKinh = request.BanKinh;
            safe.MoTa = request.MoTa;

            var createDangerZone = _childDbContext.DangerZones.Update(safe);
            if (createDangerZone == null)
            {
                throw new ApiException("Update vùng nguy hiểm thất bại");
            }
            await _childDbContext.SaveChangesAsync();

            var result = new DangerZoneResponseDTO
            {
                DangerZoneId = request.DangerZoneId,
                UserId = safe.UserId,
                ChildrenId = safe.ChildrenId,
                TenKhuVuc = request.TenKhuVuc,
                KinhDo = request.KinhDo,
                ViDo = request.ViDo,
                BanKinh = request.BanKinh,
                MoTa = request.MoTa,

            };

            return ResultDTO<DangerZoneResponseDTO>.Success(result, "Update vùng an toàn thành công");
        }

        public async Task<ResultDTO> DeleteDangerZoneAsync(Guid dangerZoneId)
        {
            var dangerZone = await _childDbContext.DangerZones.FirstOrDefaultAsync(s => s.DangerZoneId == dangerZoneId);
            if (dangerZone == null)
            {
                throw new ApiException($"Không tồn tại dangerZoneId '{dangerZoneId}' trong hệ thống!", 404);
            }

            var delete = _childDbContext.DangerZones.Remove(dangerZone);
            if (delete == null)
            {
                throw new ApiException($"Xóa dangerZoneId '{dangerZoneId}' thất bại");
            }
            await _childDbContext.SaveChangesAsync();
            return ResultDTO.Success($"Xóa vùng an toàn với dangerZoneId '{dangerZoneId}' thành công.");
        }

        public async Task<ResultDTO> DeleteDangerZoneByChildrenIdAsync(Guid childrenId)
        {
            var dangerZones = await _childDbContext.DangerZones.Where(s => s.ChildrenId == childrenId).ToListAsync();
            if (dangerZones == null || dangerZones.Count == 0)
            {
                throw new ApiException($"Không tồn tại vùng an toàn cho ChildrenId '{childrenId}' trong hệ thống!", 404);
            }

            _childDbContext.DangerZones.RemoveRange(dangerZones);
            await _childDbContext.SaveChangesAsync();
            return ResultDTO.Success($"Xóa vùng an toàn với childrenId '{childrenId}' thành công.");
        }

        public async Task<bool> CheckSafeZoneAsync(Guid childId, double lat, double lng)
        {
            // Lấy danh sách vùng an toàn của đứa trẻ
            var safeZones = await _childDbContext.SafeZones
                .Where(s => s.ChildrenId == childId)
                .ToListAsync();

            // Nếu không có vùng an toàn nào
            if (safeZones == null || safeZones.Count == 0)
            {
                return false;
            }

            // Duyệt từng vùng để kiểm tra khoảng cách
            foreach (var zone in safeZones)
            {
                double distance = CalculateDistance(lat, lng, zone.ViDo, zone.KinhDo);
                if (distance <= zone.BanKinh)
                {
                    return true; // nằm trong vùng an toàn
                }
            }

            return false; // không nằm trong vùng nào
        }

        //công thức Haversine
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Bán kính Trái Đất (m)
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
