using Microsoft.EntityFrameworkCore;
using SmartClinicQueue.Application.Common;

namespace SmartClinicQueue.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
    
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionHandlingMiddleware> _logger;
            private readonly IHostEnvironment _env;

            public ExceptionHandlingMiddleware(
                RequestDelegate next,
                ILogger<ExceptionHandlingMiddleware> logger,
                IHostEnvironment env)
            {
                _next = next;
                _logger = logger;
                _env = env;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    var (statusCode, message, isWarning) = ex switch
                    {
                        KeyNotFoundException => (
                            StatusCodes.Status404NotFound, ex.Message, true),

                        InvalidOperationException => (
                            StatusCodes.Status400BadRequest, ex.Message, true),

                        UnauthorizedAccessException => (
                            StatusCodes.Status403Forbidden, ex.Message, true),

                        ArgumentException => (
                            StatusCodes.Status400BadRequest, ex.Message, true),

                        DbUpdateException dbEx => (
                            StatusCodes.Status400BadRequest,
                            DescribeDbUpdate(dbEx),
                            true),

                        _ => (
                            StatusCodes.Status500InternalServerError,
                            _env.IsDevelopment()
                                ? ex.Message
                                : "An unexpected error occurred. Please try again later.",
                            false)
                    };

                    if (isWarning)
                        _logger.LogWarning(ex, ex.Message);
                    else
                        _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);

                    var errors = new List<string> { message };
                    if (_env.IsDevelopment() && ex.InnerException is not null)
                        errors.Add(ex.InnerException.Message);

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    var response = ApiResponse<string>.Failure(errors, message);

                    await context.Response.WriteAsJsonAsync(response);
                }
            }

            private static string DescribeDbUpdate(DbUpdateException ex)
            {
                var detail = ex.InnerException?.Message ?? ex.Message;

                if (detail.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
                    detail.Contains("FK_", StringComparison.OrdinalIgnoreCase))
                {
                    return "Related data is invalid. Please check the referenced entity and try again.";
                }

                if (detail.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                    detail.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    return "This record already exists. Please check the data and try again.";
                }

                return "Could not save data. Please check the input and try again.";
            }
        }
    }

