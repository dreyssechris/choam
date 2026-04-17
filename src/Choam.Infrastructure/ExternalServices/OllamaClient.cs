using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Choam.Application.Dtos;
using Choam.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Choam.Infrastructure.ExternalServices;

public sealed class OllamaClient(HttpClient httpClient, IConfiguration config) : IOllamaClient
{
    private string Model => config.GetValue<string>("Ollama:Model") ?? "llama3.1:8b";

    public async IAsyncEnumerable<string> StreamChatAsync(
        List<ChatMessageDto> messages,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var request = new
        {
            model = Model,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }),
            stream = true,
            options = new { temperature = 0.1 }
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await httpClient.SendAsync(
            httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(ct) is { } line)
        {
            ct.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonSerializer.Deserialize<OllamaStreamChunk>(line, JsonOptions);
            if (chunk?.Message?.Content is { Length: > 0 } content)
                yield return content;

            if (chunk?.Done == true)
                yield break;
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct)
    {
        try
        {
            using var response = await httpClient.GetAsync("/api/tags", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class OllamaStreamChunk
    {
        public OllamaStreamMessage? Message { get; set; }
        public bool Done { get; set; }
    }

    private sealed class OllamaStreamMessage
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
    }
}
