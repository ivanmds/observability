using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
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

// Configure important OpenTelemetry settings, the console exporter, and instrumentation library
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
    //loggingbuilder.IncludeFormattedMessage = true;
    //loggingbuilder.IncludeScopes = true;
    //loggingbuilder.ParseStateValues = true;
    loggingbuilder.AddOtlpExporter(opt =>
    {
        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
    });
});


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
