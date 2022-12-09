using System.Diagnostics;

namespace Bankly.Sdk.Opentelemetry.Trace
{
    public interface ITraceService
    {
        ActivitySource GetActivitySource();
        Activity StartActivity(string name = "", ActivityKind kind = ActivityKind.Internal);
    }
}