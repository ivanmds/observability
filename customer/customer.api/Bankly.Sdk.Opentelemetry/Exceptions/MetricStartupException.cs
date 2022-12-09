using System;

namespace Bankly.Sdk.Opentelemetry.Exceptions
{
    public class MetricStartupException : Exception
    {
        public MetricStartupException() 
            : base("Should be configure the AddOpenTelemetryMetrics before.") { }
    }
}
