using System.Diagnostics;
using System.Diagnostics.Metrics;
using Antifrand.Api.Repositories;
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


// Define some important constants to initialize tracing with
var serviceName = "antifraud";
var serviceVersion = "1.0.0";

Meter _meter = new Meter("Antifraud", "1.0.0");
Counter<int> _counter = _meter.CreateCounter<int>("test_request_count");
var activitySource = new ActivitySource(serviceName);

builder.Services.AddSingleton(_meter);
builder.Services.AddSingleton(_counter);
builder.Services.AddSingleton(activitySource);


builder.Services.AddOpenTelemetryMetrics(builder =>
{

    //builder.AddHttpClientInstrumentation();
    //builder.AddAspNetCoreInstrumentation();
    builder.AddMeter("Antifraud");
    builder.SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion));

    builder.AddOtlpExporter(opt =>
    {
        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        opt.Endpoint = new Uri("http://localhost:4318");
    });
});

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
         .AddOtlpExporter(opt =>
         {
             opt.Protocol = OtlpExportProtocol.HttpProtobuf;
             opt.Endpoint = new Uri("http://localhost:4318");
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
        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        opt.Endpoint = new Uri("http://localhost:4318");
    });
});


builder.Services.AddSingleton<TransactionRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
