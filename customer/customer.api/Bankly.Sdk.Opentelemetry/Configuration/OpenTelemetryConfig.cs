namespace Bankly.Sdk.Opentelemetry.Configuration
{
    public class OpenTelemetryConfig
    {
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string Endpoint { get; set; }
        public bool IsGrpc { get; set; }
        public bool EnableConsoleExporter { get; set; }
    }
}