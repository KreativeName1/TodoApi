using System.Text;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering(); // Enable buffering

        // Log the request
        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
        var requestBody = Encoding.UTF8.GetString(buffer);
        _logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path} {requestBody}");

        // Reset the request body stream position so the next middleware can read it
        context.Request.Body.Position = 0;

        await _next(context);
    }
}