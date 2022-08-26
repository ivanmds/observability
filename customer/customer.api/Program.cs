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

builder.Services.AddSingleton(_meter);
builder.Services.AddSingleton(_counter);


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
        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
    builder.AddConsoleExporter();
});

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
         .AddOtlpExporter(opt =>
         {
             opt.Protocol = OtlpExportProtocol.HttpProtobuf;
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
    });
});


var customerRepository = new CustomerRepository();
builder.Services.AddSingleton(customerRepository);

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
