using Microsoft.Extensions.Caching.Distributed;

namespace SafeLedger.API.Middlewares;

/// <summary>
/// Middleware que garantiza la idempotencia para solicitudes POST mediante el almacenamiento en caché de respuestas basado en el header Idempotency-Key.
/// Previene el procesamiento duplicado de la misma solicitud cuando ocurren reintentos.
/// </summary>
public class IdempotencyMiddleware(RequestDelegate next, IDistributedCache cache, ILogger<IdempotencyMiddleware> logger)
{
    /// <summary>
    /// Procesa la solicitud HTTP para aplicar idempotencia en operaciones POST.
    /// </summary>
    /// <param name="context">El contexto HTTP.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method != "POST")
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await next(context);
            return;
        }

        var cacheKey = $"Idempotency:{idempotencyKey}";

        var cachedResponse = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            logger.LogInformation("Idempotency activated: Returning cached response for key {Key}", idempotencyKey.ToString());
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await next(context);

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            
            await cache.SetStringAsync(cacheKey, responseText, 
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
            
            responseBody.Seek(0, SeekOrigin.Begin);
        }

        await responseBody.CopyToAsync(originalBodyStream);
    }
}