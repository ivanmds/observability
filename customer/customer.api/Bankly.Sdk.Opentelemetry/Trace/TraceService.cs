using System.Diagnostics;
using Bankly.Sdk.Opentelemetry.Exceptions;
using Bankly.Sdk.Opentelemetry.Extensions;

namespace Bankly.Sdk.Opentelemetry.Trace
{
    public sealed class TraceService : ITraceService
    {
        public ActivitySource GetActivitySource()
        {
            if (OpenTelemetryTracingExtension.ACTIVITY_SOURCE is null)
                throw new MetricStartupException();

            return OpenTelemetryTracingExtension.ACTIVITY_SOURCE;
        }

        public Activity StartActivity(string name = "", ActivityKind kind = ActivityKind.Internal)
        {
            if (OpenTelemetryTracingExtension.ACTIVITY_SOURCE is null)
                throw new MetricStartupException();
            
            return GetActivitySource().StartActivity(name, kind);
        }
    }
}
