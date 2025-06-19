using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;

    public HealthController(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    [HttpGet("redis")]
    public IActionResult CheckRedis()
    {
        try
        {
            var db = _redis.GetDatabase();
            var ping = db.Ping();

            return Ok(new
            {
                status = "connected",
                ping = ping.TotalMilliseconds + "ms"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message
            });
        }
    }
}
