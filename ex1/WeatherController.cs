using Microsoft.AspNetCore.Mvc;

namespace ex1;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var connectMessage = System.Text.Encoding.UTF8.GetBytes("data: Connected to weather stream\n\n");
        await Response.Body.WriteAsync(connectMessage);
        await Response.Body.FlushAsync();

        var random = new Random();
        var conditions = new[] { "Sunny", "Cloudy", "Rainy", "Windy", "Partly Cloudy" };

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            await Task.Delay(2000);

            var temperature = random.Next(-10, 35);
            var condition = conditions[random.Next(conditions.Length)];
            var weatherUpdate = System.Text.Encoding.UTF8.GetBytes($"data: {condition}, {temperature}°C at {DateTime.Now:HH:mm:ss}\n\n");

            await Response.Body.WriteAsync(weatherUpdate);
            await Response.Body.FlushAsync();
        }
    }
}