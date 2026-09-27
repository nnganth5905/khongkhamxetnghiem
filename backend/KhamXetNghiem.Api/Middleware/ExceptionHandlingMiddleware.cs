using System.Net;
using System.Text.Json;
using KhamXetNghiem.Api.DTOs.Responses;
using KhamXetNghiem.Api.Exceptions;

namespace KhamXetNghiem.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<
        ExceptionHandlingMiddleware
    > _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context
    )
    {
        try
        {
            await _next(
                context
            );
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(
                context,
                exception
            );
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception
    )
    {
        var status =
            exception switch
            {
                ResourceNotFoundException =>
                    HttpStatusCode.NotFound,

                BadRequestException =>
                    HttpStatusCode.BadRequest,

                ForbiddenException =>
                    HttpStatusCode.Forbidden,

                UnauthorizedAccessException =>
                    HttpStatusCode.Unauthorized,

                ArgumentException =>
                    HttpStatusCode.BadRequest,

                _ =>
                    HttpStatusCode.InternalServerError
            };

        if (
            status ==
            HttpStatusCode.InternalServerError
        )
        {
            _logger.LogError(
                exception,
                "Unhandled server exception"
            );
        }

        var message =
            status ==
            HttpStatusCode.InternalServerError
                ? "Có lỗi hệ thống xảy ra."
                : exception.Message;

        context.Response.StatusCode =
            (int)status;

        context.Response.ContentType =
            "application/json; charset=utf-8";

        var response =
            ApiResponse<object>.Error(
                message
            );

        await context.Response
            .WriteAsync(
                JsonSerializer.Serialize(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy
                                .CamelCase
                    }
                )
            );
    }
}