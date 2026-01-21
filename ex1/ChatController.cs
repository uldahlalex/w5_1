using System.Text;
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
        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        Clients.Add(Response.Body);
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
            Clients.Remove(Response.Body);
        }
    }

    [HttpPost("send")]
    public async Task SendMessage([FromBody] Message message)
    {
        var messageBytes = Encoding.UTF8.GetBytes($"data: {message.Content}\n\n");

        foreach (var client in Clients.ToArray())
        {
            try
            {
                await client.WriteAsync(messageBytes);
                await client.FlushAsync();
            }
            catch
            {
                Clients.Remove(client);
            }
        }
    }
}