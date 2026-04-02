using Backend_QLTE.Filters;
using Backend_QLTE.AuthService;
using Backend_QLTE.UserService;
using Backend_QLTE.UserService.Infrastructure.Data;
using Backend_QLTE.AuthService.Infrastructure.Data;
using Backend_QLTE.ChildService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Backend_QLTE.ChildService;
using Backend_QLTE.Hubs;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = System.Text.Encoding.UTF8;

// ✅ Add SignalR
builder.Services.AddSignalR();

// ✅ Add Controllers + Validation Filter
builder.Services.AddControllers(o => o.Filters.Add<ValidateModelAttribute>())
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);

// ✅ CORS (cho Flutter và web)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5500",      // web test (VSCode Live Server)
                "http://127.0.0.1:5500",     // web test
                "http://10.0.2.2:5074",      // Flutter emulator (HTTP)
                "https://10.0.2.2:5001",     // Flutter emulator (HTTPS)
                "http://192.168.1.100:5001",  // Thiết bị thật (LAN)
                "https://newbrasscat40.conveyor.cloud" // Dùng để test chat bên flutter
            );
    });
});

// ❌ KHÔNG đăng ký lại AddAuthentication (AuthService đã làm rồi)

// ✅ Cấu hình bổ sung cho SignalR đọc JWT từ query
builder.Services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/sosHub")))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// ✅ Swagger cấu hình như cũ
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer {token}' để xác thực"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[]{}
        }
    });
    c.CustomSchemaIds(type => type.FullName);
});

// ✅ Database contexts
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDb")));

// ✅ Thêm ChildDbContext (quan trọng!)
builder.Services.AddDbContext<ChildDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChildDb")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

// ✅ Đăng ký các microservice modules
builder.Services.AddUserService(builder.Configuration);
builder.Services.AddAuthService(builder.Configuration);
builder.Services.AddChildService(builder.Configuration);

var app = builder.Build();


// ✅ Swagger cho dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ CORS phải nằm trước Authentication và SignalR
app.UseCors("AllowFrontend");

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Gọi các pipeline microservice
app.AuthAuthService();
app.UseUserService();
app.UseChildService();
// ✅ (tùy chọn) Nếu có pipeline cho ChildService, thêm ở đây
// app.UseChildService();
app.MapHub<LocationHub>("/locationHub");
app.MapHub<SOSHub>("/sosHub");
app.MapHub<DeviceHub>("/deviceHub");
app.MapHub<StudyHub>("/studyHub");
app.MapHub<RestrictionHub>("/restrictionHub");

app.MapControllers();

// ✅ (nếu có ChatHub, bật lại dòng dưới)
app.MapHub<Backend_QLTE.Hubs.ChatHub>("/chatHub");

app.Run();
