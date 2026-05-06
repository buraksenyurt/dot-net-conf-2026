namespace InventoryMcpClient.Services;

public interface IMcpChatService
{
    IAsyncEnumerable<ChatStreamEvent> StreamChatAsync(string prompt, CancellationToken ct = default);
}

public sealed record ChatStreamEvent(string Type, string Content);
