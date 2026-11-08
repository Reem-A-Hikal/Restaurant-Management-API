using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        private static bool IsUniqueViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 2601 or 2627 };

        private static bool IsForeignKeyViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException { Number: 547 };

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(exception, "Exception after response started");
                return;
            }

            var domain = exception as DomainException;
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

                // two staff members changed the same order at the same time (needs RowVersion on Order)
                DbUpdateConcurrencyException =>
                    (HttpStatusCode.Conflict,
                    new[] { "This record was changed by someone else. Please reload and try again." }),

                // duplicate name / duplicate review / order number collision
                DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
                    (HttpStatusCode.Conflict,
                     new[] { "This action conflicts with a request that was already processed." }),

                // deleting / changing something that other records depend on
                DbUpdateException ex when IsForeignKeyViolation(ex) =>
                    (HttpStatusCode.Conflict, new[] { "This record is linked to other data and cannot be changed." }),

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
                GetMessageFromStatusCode(statusCode),
                domain?.Code,
                domain?.Params);

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
                HttpStatusCode.Conflict => "Conflict",
                _ => "Internal server error"
            };
    }
}
