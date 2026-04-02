using Microsoft.EntityFrameworkCore;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Backend_QLTE.ChildService.Application.Interfaces.Services;
using Backend_QLTE.ChildService.Infrastructure.Data.HttpClients;
using Backend_QLTE.ChildService.Infrastructure.Data.HttpClients.Interfaces;
using Backend_QLTE.ChildService.Infrastructure.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Repositories;
using Backend_QLTE.ChildService.Application.Interfaces.Orchestrators;
using Backend_QLTE.ChildService.Application.Orchestrators;
using Backend_QLTE.ChildService.Domain.Services.Interfaces;
using Backend_QLTE.ChildService.Domain.Services;
using Backend_QLTE.ChildService.Application.Services;
using Backend_QLTE.ChildService.Application.Interfaces.Mappers;
using Backend_QLTE.ChildService.Application.Mappers;
using Backend_QLTE.ChildService.Application.Factories;
using Backend_QLTE.ChildService.Application.Interfaces.Factories;
using Backend_QLTE.ChildService.Api.Middlewares;

namespace Backend_QLTE.ChildService
{
    public static class ChildServiceConfig
    {
        // Đăng ký DI cho toàn bộ tầng Application + Domain + Infrastructure
        public static IServiceCollection AddChildService(this IServiceCollection services, IConfiguration config)
        {
            /*-----------------------------------------APPLICATION--------------------------------------------*/
            // SERVICES
            services.AddScoped<IStudyPeriodService, StudyPeriodService>();
            services.AddScoped<ISOSRequestService, SOSRequestService>();
            services.AddScoped<IDeviceInfoService, DeviceInfoService>();
            services.AddScoped<IChildService, Application.Services.ChildService>();
            services.AddScoped<IZoneService, Application.Services.ZoneService>();
            services.AddScoped<ILocationService, Application.Services.LocationSerivce>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IExamScheduleService, ExamScheduleService>();
            services.AddScoped<IDeviceRestrictionService, DeviceRestrictionService>();

            // FACTORIES
            services.AddScoped<IStudyPeriodFactory, StudyPeriodFactory>();
            services.AddScoped<ISOSRequestFactory, SOSRequestFactory>();
            services.AddScoped<IDeviceInfoFactory, DeviceInfoFactory>();
            services.AddScoped<IScheduleFactory, ScheduleFactory>();
            services.AddScoped<IExamScheduleFactory, ExamScheduleFactory>();

            // MAPPERS
            services.AddScoped<IStudyPeriodResponseMapper, StudyPeriodResponseMapper>();
            services.AddScoped<ISOSRequestResponseMapper, SOSRequestResponseMapper>();
            services.AddScoped<IDeviceInfoResponseMapper, DeviceInfoResponseMapper>();
            services.AddScoped<IScheduleResponseMapper, ScheduleResponseMapper>();
            services.AddScoped<IExamScheduleResponseMapper, ExamScheduleResponseMapper>();

            // ORCHESTRATORS
            services.AddScoped<IStudyPeriodOrchestrator, StudyPeriodOrchestrator>();
            services.AddScoped<ISOSRequestOrchestrator, SOSRequestOrchestrator>();
            services.AddScoped<IDeviceInfoOrchestrator, DeviceInfoOrchestrator>();
            services.AddScoped<IScheduleOrchestrator, ScheduleOrchestrator>();
            services.AddScoped<IExamScheduleOrchestrator, ExamScheduleOrchestrator>();

            // VALIRATORS
            // Dịch vụ validation chung

            // Đăng ký tất cả validators tự động
            //services.Scan(scan => scan
            //    .FromAssemblies(typeof(RegisterUserValidator).Assembly) // Quét tất cả các lớp trong assembly chứa RegisterUserValidator 
            //    .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))// Đăng ký tất cả các lớp triển khai IValidator<>
            //    .AsImplementedInterfaces()// Đăng ký chúng theo giao diện mà chúng triển khai
            //    .WithScopedLifetime()); // Sử dụng phạm vi Scoped cho các dịch vụ này

            /*-----------------------------------------DOMAIN--------------------------------------------*/
            // DOMAIN SERVICES
            services.AddScoped<IStudyPeriodDomainService, StudyPeriodDomainService>();
            services.AddScoped<ISOSRequestDomainService, SOSRequestDomainService>();
            services.AddScoped<IDeviceInfoDomainService, DeviceInfoDomainService>();
            services.AddScoped<IScheduleDomainService, ScheduleDomainService>();
            services.AddScoped<IExamScheduleDomainService, ExamScheduleDomainService>();

            //MAPPERS
            services.AddScoped<IPaginationMapper, PaginationMapper>();

            /*-----------------------------------------INFRASTRUCTURE--------------------------------------------*/
            //CONFIG

            // HTTP Client
            // HTTP Client
            services.AddHttpClient<IUserServiceClient, UserServiceClient>("Membership_UserServiceClient", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var userServiceUrl = config["ServiceUrls:UserService"];
                client.BaseAddress = new Uri(userServiceUrl);
            });

            // Configure database contexts
            services.AddDbContext<ChildDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("ChildDb")));

            // REPOSITORIES
            services.AddScoped<IStudyPeriodRepository, StudyPeriodRepository>();
            services.AddScoped<ISOSRequestRepository, SOSRequestRepository>();
            services.AddScoped<IChildRepository, ChildRepository>();
            services.AddScoped<IDeviceInfoRepository, DeviceInfoRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IExamScheduleRepository, ExamScheduleRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Unit of Work
            return services;
        }

        // Middleware pipeline riêng cho ChildService
        public static IApplicationBuilder UseChildService(this IApplicationBuilder app)
        {
            // Nếu sau này bạn có middleware riêng (exception handler, logger, v.v.)
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            return app;
        }
    }
}
