using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;

namespace Ca.Backend.Test.API.Middlewares;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (ValidationException e)
        {
            var response = new GenericHttpResponse
            {
                Errors = e.Errors.Select(error => error.ErrorMessage),
                StatusCode = StatusCodes.Status400BadRequest,
                Data = null
            };

            _logger.LogWarning("Validation exception occurred: {Errors}", response.Errors);
            await BuildResponseAsync(context, response.StatusCode, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
        }
        catch (ApplicationException ex)
        {
            var response = new GenericHttpResponse
            {
                Errors = new[] { ex.Message },
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = null
            };

            _logger.LogError(ex, "Application exception occurred: {Message}", ex.Message);
            await BuildResponseAsync(context, response.StatusCode, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
        }
        catch (Exception ex)
        {
            var response = new GenericHttpResponse
            {
                Errors = new[] { "An unexpected error occurred." },
                StatusCode = StatusCodes.Status500InternalServerError,
                Data = null
            };

            _logger.LogError(ex, "An unexpected exception occurred: {Message}", ex.Message);
            await BuildResponseAsync(context, response.StatusCode, JsonSerializer.Serialize(response), MediaTypeNames.Application.Json);
        }
    }

    private async Task BuildResponseAsync(HttpContext context, int statusCode, string body, string contentType)
    {
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = contentType;
        await context.Response.WriteAsync(body);
    }
}

public class GenericHttpResponse<T>
{
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public IEnumerable<string> Errors { get; set; }

    public GenericHttpResponse()
    {
        TraceId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
        Errors = Enumerable.Empty<string>();
    }
}

public class GenericHttpResponse
{
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }

    [JsonPropertyName("data")]
    public object? Data { get; set; }

    [JsonPropertyName("errors")]
    public IEnumerable<string> Errors { get; set; }

    public GenericHttpResponse()
    {
        TraceId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
        Errors = Enumerable.Empty<string>();
    }
}



