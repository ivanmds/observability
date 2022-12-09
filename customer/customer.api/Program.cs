using Bankly.Sdk.Opentelemetry.Configuration;
using Bankly.Sdk.Opentelemetry.Extensions;
using customer.api.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var service = builder.Services;


// Define some important constants to initialize tracing with

var otlpConfig = new OpenTelemetryConfig();
otlpConfig.ServiceName = "Customer";
otlpConfig.ServiceVersion = "1.0.1";
otlpConfig.Endpoint = "http://localhost:4317";
otlpConfig.IsGrpc = true;
otlpConfig.EnableConsoleExporter = true;

builder.Services.AddBanklyOpenTelemetryMetrics(otlpConfig);
//builder.Services.AddBanklyOpenTelemetryTracing(otlpConfig);
//builder.Logging.AddBanklyOpenTelemetryLogging(otlpConfig);


//builder.Services.AddSingleton<CustomerRepository>();

var app = builder.Build();

app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI();

app.UseBanklyMetrics();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

