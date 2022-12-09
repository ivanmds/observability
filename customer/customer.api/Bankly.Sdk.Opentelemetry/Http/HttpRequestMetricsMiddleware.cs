using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Bankly.Sdk.Opentelemetry.Extensions;
using Bankly.Sdk.Opentelemetry.Metric;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Bankly.Sdk.Opentelemetry.Http
{
    internal class HttpRequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpRequestMetricsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            StringValues correlationIdValue = default;
            context.Request.Headers.TryGetValue(ConstValues.KEY_CORRELATION_ID, out correlationIdValue);

            var correlationId = correlationIdValue.ToString();
            if (string.IsNullOrEmpty(correlationId))
                correlationId = "none";

            var activity = Activity.Current;

            Activity.Current?.SetTag(ConstValues.KEY_CORRELATION_ID, correlationId);

            try
            {
                var watch = Stopwatch.StartNew();

                OpenTelemetryMetricsExtension.HTTP_REQUEST_COUNTER.Add(1);

                await _next(context);

                var statusCode = context.Response.StatusCode;

                if (statusCode >= 200 && statusCode <= 299)
                    OpenTelemetryMetricsExtension.HTTP_REQUEST_200_COUNTER.Add(1);
                else if (statusCode >= 400 && statusCode <= 499)
                    OpenTelemetryMetricsExtension.HTTP_REQUEST_400_COUNTER.Add(1);
                else if (statusCode >= 500)
                    OpenTelemetryMetricsExtension.HTTP_REQUEST_500_COUNTER.Add(1);

                watch.Stop();
                var elapsedTime = (int)watch.ElapsedMilliseconds;
                OpenTelemetryMetricsExtension.HTTP_REQUEST_ELAPSED_TIME.Record(elapsedTime);
            }
            catch (Exception ex)
            {
                var exception = ex.InnerException ?? ex;
                var tagExceptionName = MetricService.CreateTag(ConstValues.EXCEPTION_FULL_NAME, exception.GetType().FullName);
                OpenTelemetryMetricsExtension.HTTP_REQUEST_500_COUNTER.Add(1, tagExceptionName);
                throw ex;
            }
        }
    }
}
