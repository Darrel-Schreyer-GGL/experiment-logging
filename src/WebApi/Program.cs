using MongoDB.Driver;
using OpenTelemetry.Logs;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(builder =>
{
    builder.AddOpenTelemetry(options =>
    {
        options.AddConsoleExporter();
        options.AddOtlpExporter(oltp =>
        {
            oltp.Endpoint = new Uri("http://localhost:4317");
        });

    });

    Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.MongoDB(
                databaseUrl: "mongodb://localhost:27017/LogsDb",
                collectionName: "DomainEvents",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                batchPostingLimit: 10,
                period: TimeSpan.FromSeconds(5))
            .WriteTo.File(
                formatter: new JsonFormatter(),
                path: "logs/domain_events-.json",
                rollingInterval: RollingInterval.Hour,
                restrictedToMinimumLevel: LogEventLevel.Information,
                buffered: true,
                flushToDiskInterval: TimeSpan.FromSeconds(30),
                fileSizeLimitBytes: 10_000_000,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 100)
            .CreateLogger();

    builder.AddSerilog();
});

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient("mongodb://localhost:27017"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("LogsDb"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
