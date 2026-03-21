using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
namespace GenReport.Middlewares
{
    public class PerformanceInspector(ILogger<PerformanceInspector> logger) : IEndpointFilter
    {
        private readonly ILogger<PerformanceInspector> _logger = logger;
        private static readonly ConcurrentDictionary<string, (long totalTime, int requestCount)> _routeStats = new();

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = await next(context);
            stopWatch.Stop();
            var route = context.HttpContext.Request.Path.Value;

            // Calculate average time taken for this route
            _routeStats.AddOrUpdate(route!, (stopWatch.ElapsedMilliseconds, 1), (_, data) =>
            {
                var (totalTime, requestCount) = data;
                return (totalTime + stopWatch.ElapsedMilliseconds, requestCount + 1);
            });
            if (context.HttpContext.Response.StatusCode != 200)
            {
                return result;
            }

            {
                // Log the performance data for the route
                var (averageTime, requestCount) = _routeStats[route!];

                // Foramt for log data
                var logData = $"Route: {route}, Time: {stopWatch.ElapsedMilliseconds}ms Average Time: {averageTime / requestCount}ms, Total Requests: {requestCount}";
                // Log the performance data using Serilog
                Log.Information(logData);
            }
            return result;  
        }
    }
}
