using System.Text;

namespace SheetService.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await LogRequest(context);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                await LogResponse(context);
            }
            finally
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                string body = string.Empty;
                if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
                {
                    using var reader = new StreamReader(
                        context.Request.Body,
                        Encoding.UTF8,
                        detectEncodingFromByteOrderMarks: false,
                        leaveOpen: true
                    );
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                var logMessage = $@"
+--------------------------------------------------------------------+
|                         INCOMING REQUEST                           |
+--------------------+-----------------------------------------------+
| Time               | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
| Method             | {context.Request.Method}
| Path               | {context.Request.Path}
| Query              | {context.Request.QueryString}
| IP                 | {context.Connection.RemoteIpAddress}
+--------------------+-----------------------------------------------+
| REQUEST BODY:
{FormatJson(body)}
+--------------------------------------------------------------------+";

                _logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging request");
            }
        }

        private async Task LogResponse(HttpContext context)
        {
            try
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var logMessage = $@"
+--------------------------------------------------------------------+
|                         OUTGOING RESPONSE                          |
+--------------------+-----------------------------------------------+
| Time               | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC
| Status             | {context.Response.StatusCode}
| Path               | {context.Request.Path}
+--------------------+-----------------------------------------------+
| RESPONSE BODY:
{FormatJson(body)}
+--------------------------------------------------------------------+";

                _logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging response");
            }
        }

        private string FormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "  (empty)";

            try
            {
                var parsedJson = System.Text.Json.JsonDocument.Parse(json);
                var formatted = System.Text.Json.JsonSerializer.Serialize(parsedJson, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                var lines = formatted.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = "  " + lines[i];
                }
                return string.Join('\n', lines);
            }
            catch
            {
                return $"  {json}";
            }
        }
    }
}