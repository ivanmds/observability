using System.Diagnostics;
using System.Diagnostics.Metrics;
using customer.api.Repository;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var service = builder.Services;


// Define some important constants to initialize tracing with
var serviceName = "customers";
var serviceVersion = "1.0.0";

Meter _meter = new Meter("Customer", "1.0.0");
Counter<int> _counter = _meter.CreateCounter<int>("test_request_count");
var activitySource = new ActivitySource(serviceName);

builder.Services.AddSingleton(_meter);
builder.Services.AddSingleton(_counter);
builder.Services.AddSingleton(activitySource);

string uri = Environment.GetEnvironmentVariable("COLLECTOR_URI") ?? "http://localhost:4318";

var isGrpcValue = Environment.GetEnvironmentVariable("IS_GRPC");
bool isGrpc = isGrpcValue == "YES" ? true : false;

Console.WriteLine(uri);
Console.WriteLine(isGrpc);

builder.Services.AddOpenTelemetryMetrics(builder =>
{
    //builder.AddHttpClientInstrumentation();
    //builder.AddAspNetCoreInstrumentation();
    builder.AddMeter("Customer");
    builder.SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion));

    builder.AddOtlpExporter(opt =>
    {
        opt.Protocol = isGrpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
        opt.Endpoint = new Uri(uri);
        
    });

    builder.AddConsoleExporter();
});


builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
         .AddOtlpExporter(opt =>
         {
             opt.Protocol = isGrpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
             opt.Endpoint = new Uri(uri);
         })
        .AddSource(serviceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation();

});


builder.Logging.AddOpenTelemetry(loggingbuilder =>
{
    loggingbuilder.AddOtlpExporter(opt =>
    {
        opt.Protocol = isGrpc ? OtlpExportProtocol.Grpc : OtlpExportProtocol.HttpProtobuf;
        opt.Endpoint = new Uri(uri);
    });
    loggingbuilder.AddConsoleExporter();
});


builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
