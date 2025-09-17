using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Resources;
using Resources.Data.Resources;
using System.Threading.Tasks;

namespace API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _getLimit = 600;
        private readonly int _modifyLimit = 15; // Limit for POST and DELETE
        private readonly TimeSpan _period = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string key = GetClientKey(context);

            if (string.IsNullOrEmpty(key))
            {
                await _next(context);
                return;
            }

            // Determine rate limit based on HTTP method
            int requestLimit = context.Request.Method switch
            {
                "GET" => _getLimit,
                "POST" => _modifyLimit,
                "PUT" => _modifyLimit,
                "DELETE" => _modifyLimit,
                _ => _modifyLimit // Default to _getLimit for other methods
            };

            // Add method to key to track separately for each HTTP method type
            string cacheKey = $"{key}:{context.Request.Method}";

            // Check request count in cache
            var attempts = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _period;
                return 0;
            });

            if (attempts >= requestLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync(NotifiAndAlertsResources.RateLimitExceeded);
                return;
            }

            // Increment the request count and save back to cache
            _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(1));
            await _next(context);
        }

        private string GetClientKey(HttpContext context)
        {
            // Use authenticated user ID if available, otherwise use IP address
            return context.User.Identity.IsAuthenticated
                ? context.User.Identity.Name
                : context.Connection.RemoteIpAddress?.ToString();
        }
    }
}
