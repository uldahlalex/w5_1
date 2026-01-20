using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ex1;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private static readonly ConcurrentBag<Stream> Clients = new();

    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        await Response.Body.FlushAsync();

        Clients.Add(Response.Body);

        try
        {
            var connectMessage = System.Text.Encoding.UTF8.GetBytes("data: Connected to chat\n\n");
            await Response.Body.WriteAsync(connectMessage);
            await Response.Body.FlushAsync();

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
        finally
        {
            Clients.TryTake(out _);
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] Message message)
    {
        var clientsSnapshot = Clients.ToArray();

        foreach (var client in clientsSnapshot)
        {
            try
            {
                var messageBytes = System.Text.Encoding.UTF8.GetBytes($"data: {message.Content}\n\n");
                await client.WriteAsync(messageBytes);
                await client.FlushAsync();
            }
            catch
            {
                Clients.TryTake(out _);
            }
        }

        return Ok(new { sent = clientsSnapshot.Length });
    }
}