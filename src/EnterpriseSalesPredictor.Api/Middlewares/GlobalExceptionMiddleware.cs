using Microsoft.AspNetCore.Mvc;
using EnterpriseSalesPredictor.Application.Validators;

namespace EnterpriseSalesPredictor.Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception exception)
        {
            if (exception is ValidationException validationException)
            {
                var validationProblem = new ValidationProblemDetails(
                    validationException.Errors
                        .GroupBy(error => error.Field)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.Message).ToArray()))
                {
                    Title = "Validation failed.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "One or more validation errors occurred.",
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(validationProblem);
                return;
            }

            _logger.LogError(exception, "Unhandled exception for request {Path}", context.Request.Path);

            var problem = new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The request could not be processed.",
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
