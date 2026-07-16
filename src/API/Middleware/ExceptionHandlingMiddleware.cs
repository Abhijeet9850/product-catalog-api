using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using FluentValidation;

namespace API.Middleware
{
    public class ErrorResponse
    {
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string? Detail { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
        public string TraceId { get; set; } = string.Empty;
    }

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception processing request {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new ErrorResponse { TraceId = context.TraceIdentifier };

            (HttpStatusCode statusCode, string title) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                ValidationException => (HttpStatusCode.BadRequest, "Validation failed"),
                ValidationAppException => (HttpStatusCode.BadRequest, "Validation failed"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            response.Title = title;
            response.Status = (int)statusCode;
            response.Detail = exception is ValidationException ve
                ? string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))
                : exception.Message;

            if (exception is ValidationException validationException)
            {
                response.Errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.Status;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}