using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogReportController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<LogReportController> _logger;

    public LogReportController(IMongoDatabase database, ILogger<LogReportController> logger)
    {
        _database = database;
        _logger = logger;
    }

    [HttpGet("last-logs/{category}/{n:int}")]
    public IActionResult GetLastNLogs(string category, uint n)
    {
        var collection = _database.GetCollection<BsonDocument>("DomainEvents");
        // Filter by Category if provided, otherwise get all logs
        var filter = category != null
            ? Builders<BsonDocument>.Filter.Eq("Properties.Category", category)
            : FilterDefinition<BsonDocument>.Empty;

        var lastLogs = collection.Find(filter)
            .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
            .Limit((int)n)
            .ToList();

        if (lastLogs == null)
        {
            _logger.LogWarning("No logs found in DomainEvents collection.");
            return NotFound("No logs available.");
        }

        return Ok(lastLogs.ToJson());
    }

    [HttpGet("last-logs/weather/{n:int}")]
    public IActionResult GetLastNWeatherLogs(uint n)
    {
        var collection = _database.GetCollection<BsonDocument>("DomainEvents");
        // Filter by Category if provided, otherwise get all logs
        var filter = Builders<BsonDocument>.Filter.Eq("Properties.Category", "WeatherLog");

        var lastLogs = collection.Find(filter)
            .Sort(Builders<BsonDocument>.Sort.Descending("Timestamp"))
            .Limit((int)n)
            .ToList();

        if (lastLogs == null)
        {
            _logger.LogWarning("No logs found in DomainEvents collection.");
            return NotFound("No logs available.");
        }

        var jsonWriterSettings = new JsonWriterSettings { Indent = true };
        return Ok(lastLogs.ToJson(jsonWriterSettings));
    }
}
