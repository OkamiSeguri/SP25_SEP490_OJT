using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FOMSOData.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RequestTracker> RequestCounts = new();

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                // Kiểm tra giới hạn request theo IP
                if (!IsAllowed(ipAddress, out string rateLimitMessage))
                {
                    throw new RateLimitException(rateLimitMessage);
                }

                await _next(context);
            }
            catch (RateLimitException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.TooManyRequests, "Too many requests", ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, "Bad request", ex.Message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Internal server error", ex.Message);
            }
        }

        private static bool IsAllowed(string ip, out string message)
        {
            var now = DateTime.UtcNow;
            var tracker = RequestCounts.GetOrAdd(ip, _ => new RequestTracker(now));

            lock (tracker)
            {
                if (now - tracker.LastReset >= TimeSpan.FromSeconds(10))
                {
                    tracker.RequestCount = 0;
                    tracker.LastReset = now;
                }

                if (tracker.RequestCount >= 10) // Giới hạn 10 requests/10 giây
                {
                    message = $"Rate limit exceeded. Try again after {10 - (now - tracker.LastReset).Seconds} seconds.";
                    return false;
                }

                tracker.RequestCount++;
                message = string.Empty;
                return true;
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string detail, string errorMessage = "")
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                code = context.Response.StatusCode,
                detail,
                error = string.IsNullOrEmpty(errorMessage) ? null : errorMessage
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    public class RateLimitException : Exception
    {
        public RateLimitException(string message) : base(message) { }
    }

    class RequestTracker
    {
        public int RequestCount { get; set; }
        public DateTime LastReset { get; set; }

        public RequestTracker(DateTime resetTime)
        {
            RequestCount = 0;
            LastReset = resetTime;
        }
    }
}
