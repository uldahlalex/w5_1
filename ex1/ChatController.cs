using Microsoft.AspNetCore.Mvc;

namespace ex1;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private static readonly List<Stream> Clients = new();

    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        await Response.Body.FlushAsync();

        lock (Clients)
        {
            Clients.Add(Response.Body);
        }

        var connectMessage = System.Text.Encoding.UTF8.GetBytes("data: Connected to chat\n\n");
        await Response.Body.WriteAsync(connectMessage);
        await Response.Body.FlushAsync();

        try
        {
            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
        finally
        {
            lock (Clients)
            {
                Clients.Remove(Response.Body);
            }
        }
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] Message message)
    {
        var messageBytes = System.Text.Encoding.UTF8.GetBytes($"data: {message.Content}\n\n");

        Stream[] clientsSnapshot;
        lock (Clients)
        {
            clientsSnapshot = Clients.ToArray();
        }

        foreach (var client in clientsSnapshot)
        {
            try
            {
                await client.WriteAsync(messageBytes);
                await client.FlushAsync();
            }
            catch
            {
                lock (Clients)
                {
                    Clients.Remove(client);
                }
            }
        }

        return Ok(new { sent = clientsSnapshot.Length });
    }
}