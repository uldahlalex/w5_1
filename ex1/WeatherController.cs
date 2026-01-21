using Microsoft.AspNetCore.Mvc;

namespace ex1;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        
        while (true)
        {
            await Task.Delay(2000);
            var weatherUpdate = System.Text.Encoding.UTF8.GetBytes($"it is sunny and {new Random().Next()} degrees at {DateTime.Now.ToLocalTime()}\n\n");
            await Response.Body.WriteAsync(weatherUpdate);
            await Response.Body.FlushAsync();
        }
    }
}