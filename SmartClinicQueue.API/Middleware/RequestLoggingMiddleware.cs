using SmartClinicQueue.Domain.Entities;
using SmartClinicQueue.Infrastructure.Persistance;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace SmartClinicQueue.API.Middleware
{
    
     
        public class RequestLoggingMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly IServiceScopeFactory _scopeFactory;
            private readonly ILogger<RequestLoggingMiddleware> _logger;

            public RequestLoggingMiddleware(
                RequestDelegate next,
                IServiceScopeFactory scopeFactory,
                ILogger<RequestLoggingMiddleware> logger)
            {
                _next = next;
                _scopeFactory = scopeFactory;
                _logger = logger;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider
                            .GetRequiredService<ApplicationDbContext>();

                        var log = new RequestLog
                        {
                            HttpMethod = context.Request.Method,
                            Url = context.Request.Path + context.Request.QueryString,
                            Headers = GetRelevantHeaders(context),
                            IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                            StatusCode = context.Response.StatusCode,
                            ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                            UserId = GetUserId(context),
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        dbContext.RequestLogs.Add(log);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save RequestLog");
                    }
                }
            }

            private static int? GetUserId(HttpContext context)
            {
                if (context.User?.Identity?.IsAuthenticated != true)
                    return null;

                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return int.TryParse(userIdClaim, out var userId) ? userId : null;
            }

            private static string? GetRelevantHeaders(HttpContext context)
            {
                var headers = new Dictionary<string, string>();

                foreach (var header in context.Request.Headers)
                {
                    if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                        continue;

                    headers[header.Key] = header.Value.ToString();
                }

                return headers.Count == 0
                    ? null
                    : JsonSerializer.Serialize(headers);
            }
        }
    }

