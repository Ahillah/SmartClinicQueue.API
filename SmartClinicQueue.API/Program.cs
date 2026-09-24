using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartClinicQueue.API.Hubs;
using SmartClinicQueue.API.Middleware;
using SmartClinicQueue.API.Services;
using SmartClinicQueue.Application.Interfaces.IReposirories;
using SmartClinicQueue.Application.Interfaces.IServices;
using SmartClinicQueue.Application.Services;
using SmartClinicQueue.Domain.Entities;
using SmartClinicQueue.Infrastructure.Persistance;
using SmartClinicQueue.Infrastructure.Repositories;
using SmartClinicQueue.Infrastructure.Services;
using System.Text;

namespace SmartClinicQueue.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("AuthPolicy", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueLimit = 0;
                });
            });
            // Application Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped(
   typeof(IGenericRepository<,>),
   typeof(GenericRepository<,>));
            builder.Services.AddScoped<IQueueTicketRepository, QueueTicketRepository>();
            builder.Services.AddScoped<IQueueCleanupService, QueueCleanupService>();

            builder.Services.AddScoped<
                IBackgroundJobScheduler,
                HangfireBackgroundJobScheduler>();
            builder.Services.AddHangfire(config =>
    config
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            builder.Configuration.GetConnectionString("HangfireConnection")));

            builder.Services.AddHangfireServer();


            // MediatR
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(IAuthService).Assembly));
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IQueueNotificationService, QueueNotificationService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // Authentication must come before Authorization
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            // Custom Middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.MapHub<QueueHub>("/hubs/queue");
            app.MapControllers();
            using (var scope = app.Services.CreateScope())
            {
                var scheduler =
                    scope.ServiceProvider
                        .GetRequiredService<IBackgroundJobScheduler>();

                scheduler.ScheduleDailyQueueCleanup(
                    service => service.CleanupExpiredQueueAsync(),
                    Cron.Daily(0, 0));
            }
            app.Run();
        }
    }
}