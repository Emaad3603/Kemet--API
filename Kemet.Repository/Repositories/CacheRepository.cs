using Kemet.APIs.Helpers;
using Kemet.Core.Repositories.InterFaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CacheRepository : ICacheRepository
{
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheRepository(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve // Or Preserve if needed
        };

        _jsonOptions.Converters.Add(new DateOnlyConverter());
    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _database.StringGetAsync(key);
        return !value.IsNullOrEmpty ? value : default;
    }

    public async Task SetAsync(string key, object value, TimeSpan duration)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await _database.StringSetAsync(key, json, duration);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}
