using ElectroPiTask.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace ElectroPiTask.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException notFound:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = notFound.Message;
                    _logger.LogWarning(notFound, "Not found: {Message}", notFound.Message);
                    break;

                case BusinessException business:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = business.Message;
                    _logger.LogWarning(business, "Business rule violation: {Message}", business.Message);
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    // Hide internal details in production to avoid leaking stack traces
                    message = _env.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred. Please try again later.";
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            var response = new
            {
                success = false,
                message,
                data = (object?)null,
                errors = new List<string>()
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
