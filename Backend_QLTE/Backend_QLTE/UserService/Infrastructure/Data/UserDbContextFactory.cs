using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Backend_QLTE.UserService.Infrastructure.Data
{
    public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            // 🔍 Tìm lên trên cho tới khi gặp appsettings.json
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            DirectoryInfo? rootWithAppsettings = null;

            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, "appsettings.json");
                if (File.Exists(candidate))
                {
                    rootWithAppsettings = current;
                    break;
                }
                current = current.Parent;
            }

            // Nếu không tìm thấy, ném lỗi rõ ràng
            if (rootWithAppsettings == null)
                throw new FileNotFoundException("Không tìm thấy appsettings.json khi tạo UserDbContext (design-time).");

            var config = new ConfigurationBuilder()
                .SetBasePath(rootWithAppsettings.FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var cs = config.GetConnectionString("UserDb");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidDataException("ConnectionStrings:UserDb trống hoặc không tồn tại trong appsettings.json.");

            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseSqlServer(cs)
                .Options;

            return new UserDbContext(options);
        }
    }
}
