using Microsoft.AspNetCore.Http;
using Rest.API.Responses;
using Rest.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Rest.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch(Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errors) = exception switch
            {
                NotFoundException ex =>
                    (HttpStatusCode.NotFound, new[] { ex.Message }),

                // 400 - Business rule violated
                BusinessException ex =>
                    (HttpStatusCode.BadRequest, new[] { ex.Message }),

                // 400 - Validation failed
                ValidationException ex =>
                    (HttpStatusCode.BadRequest, ex.Errors.ToArray()),

                // 403 - Not authorized
                ForbiddenException ex =>
                    (HttpStatusCode.Forbidden, new[] { ex.Message }),

                ApplicationException ex =>
                    (HttpStatusCode.BadRequest, new[] { ex.Message }),

                // 500 - Anything else is unexpected
                _ => (HttpStatusCode.InternalServerError,
                     new[] { "An unexpected error occurred." })
            };

            if(statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unexpected error: {Message}", exception.Message);
            else
                _logger.LogWarning(exception, "Handled error: {Message}", exception.Message);

            var response = ApiResponse<string>.FailResponse(
                errors.ToList(),
                GetMessageFromStatusCode(statusCode));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        private static string GetMessageFromStatusCode(HttpStatusCode statusCode) =>
            statusCode switch
            {
                HttpStatusCode.NotFound => "Resource not found",
                HttpStatusCode.BadRequest => "Bad request",
                HttpStatusCode.Forbidden => "Access denied",
                _ => "Internal server error"
            };
    }
}
