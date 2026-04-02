using Backend_QLTE.AuthService.Application.Interfaces.Repositories;
using Backend_QLTE.AuthService.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Backend_QLTE.AuthService.Infrastructure.Repositories
{
    public class InMemoryOtpRepository : IOtpRepository
    {
        private readonly IMemoryCache _cache;

        public InMemoryOtpRepository(IMemoryCache cache)
        {
            _cache = cache;
        }
        private string BuildKey(string userId, string type) => $"otp:{type}:{userId}";

        public Task SaveAsync(Otp otp, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(otp.UserId, otp.Type);
            _cache.Set(key, otp, otp.Expiry); // set TTL = thời gian hết hạn của OTP
            return Task.CompletedTask;
        }

        public Task<Otp?> GetAsync(string userId, string type, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(userId, type);
            _cache.TryGetValue<Otp>(key, out var otp);
            return Task.FromResult(otp);
        }

        public Task RemoveAsync(string userId, string type, CancellationToken cancellationToken = default)
        {
            var key = BuildKey(userId, type);
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
