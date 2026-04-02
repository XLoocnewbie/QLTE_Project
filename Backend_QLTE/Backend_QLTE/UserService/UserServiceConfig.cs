using Backend_QLTE.UserService.Application.Interfaces.Repositories;
using Backend_QLTE.UserService.Application.Interfaces.Services;
using Backend_QLTE.UserService.Application.Services;
using Backend_QLTE.UserService.Domain.Entities;
using Backend_QLTE.UserService.Domain.Services.Interfaces;
using Backend_QLTE.UserService.Domain.Services;
using Backend_QLTE.UserService.Infrastructure.Config;
using Backend_QLTE.UserService.Infrastructure.Data;
using Backend_QLTE.UserService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend_QLTE.UserService.Application.Validators.Users;
using Backend_QLTE.UserService.Application.Interfaces.Validators;
using Backend_QLTE.UserService.Application.Interfaces.Orchestrators;
using Backend_QLTE.UserService.Application.Orchestrators;
using Backend_QLTE.UserService.Api.Middlewares;
using Backend_QLTE.UserService.Application.Interfaces.Mappers;
using Backend_QLTE.UserService.Application.Mappers;
using static System.Net.Mime.MediaTypeNames;

namespace Backend_QLTE.UserService
{
    public static class UserServiceConfig
    {
        public static IServiceCollection AddUserService(this IServiceCollection services, IConfiguration config)
        {

            /*-----------------------------------------APPLICATION--------------------------------------------*/
            // SERVICES
            services.AddScoped<IUserService, Application.Services.UserService>(); // User Service
            services.AddScoped<IRoleService, RoleService>(); // Role Service

            // VALIRATORS
            // Dịch vụ validation chung
            services.AddScoped<IValidationService, ValidationService>(); // UserService

            // ORCHESTRATORS
            services.AddScoped<IUserOrchestrator, UserOrchestrator>(); // User Orchestrator
            services.AddScoped<IRoleOrchestrator, RoleOrchestrator>(); // Role Orchestrator

            // FACTORIES    
            services.AddScoped<Application.Interfaces.Factories.IUserFactory, Application.Factories.UserFactory>();
            services.AddScoped<Application.Interfaces.Factories.IRoleFactory, Application.Factories.RoleFactory>();

            // MAPPERS
            services.AddScoped<IUserResponseMapper, UserResponseMapper>(); // User Mapper
            services.AddScoped<IPaginationMapper, PaginationMapper>();
            services.AddScoped<IRoleResponseMapper, RoleResponseMapper>(); // Role Mapper

            // Đăng ký tất cả validators tự động
            services.Scan(scan => scan
                .FromAssemblies(typeof(RegisterUserValidator).Assembly) // Quét tất cả các lớp trong assembly chứa RegisterUserValidator 
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))// Đăng ký tất cả các lớp triển khai IValidator<>
                .AsImplementedInterfaces()// Đăng ký chúng theo giao diện mà chúng triển khai
                .WithScopedLifetime()); // Sử dụng phạm vi Scoped cho các dịch vụ này


            /*-----------------------------------------DOMAIN--------------------------------------------*/

            // DOMAINSERVICES
            services.AddScoped<IUserDomainService, UserDomainService>(); // User Domain Service
            services.AddScoped<IRoleDomainService, RoleDomainService>(); // Role Domain Service

            // GENERATE USERNAME GUID
            services.AddScoped<IGuidUserNameGenerator, GuidUserNameGenerator>(); // Tạo username ngẫu nhiên


            /*-----------------------------------------INFRASTRUCTURE--------------------------------------------*/
            //CONFIG
            // Config ánh xạ UserSetting vs model UserSetting
            services.Configure<UserSettings>(config.GetSection("UserSettings"));

            // Configure database contexts
            // UserService
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("UserDb")));

            // IDENTITY
            // Cấu hình Identity để quản lý người dùng (User) và vai trò (Role).
            services.AddIdentityCore<User>(options => { })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();

            // REPOSITORIES
            services.AddScoped<IRoleRepository, RoleRepository>(); // Role Repository
            services.AddScoped<IUserRepository, UserRepository>(); // User Repository
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Base Repository

            return services;
        }

        public static IApplicationBuilder AuthAuthService(this IApplicationBuilder app)
        {
            // Đăng ký middleware riêng của UserService
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}
