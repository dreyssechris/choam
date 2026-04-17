using System.Text;
using System.Text.Json;
using Choam.Application.Dtos;
using Choam.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Choam.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("chat")]
    [RequestSizeLimit(32 * 1024)]
    public async Task SendMessage(ChatRequestDto request, CancellationToken ct)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        var fullResponse = new StringBuilder();

        await foreach (var token in chatService.StreamResponseAsync(request, ct))
        {
            fullResponse.Append(token);
            await WriteEventAsync("token", new { content = token }, ct);
        }

        var proposal = await chatService.TryParseProposalAsync(
            fullResponse.ToString(), ct);

        await WriteEventAsync("done", new { proposal }, ct);
    }

    [AllowAnonymous]
    [HttpGet("available")]
    public async Task<ActionResult<bool>> IsAvailable(CancellationToken ct)
        => Ok(await chatService.IsAvailableAsync(ct));

    private async Task WriteEventAsync(string eventType, object data, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await Response.WriteAsync($"event: {eventType}\ndata: {json}\n\n", Encoding.UTF8, ct);
        await Response.Body.FlushAsync(ct);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
