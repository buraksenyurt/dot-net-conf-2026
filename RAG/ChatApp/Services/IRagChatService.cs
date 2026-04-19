using ChatApp.Models;

namespace ChatApp.Services;

public interface IRagChatService
{
    Task<List<DocumentChunk>> SearchAsync(string query, CancellationToken ct = default);

    IAsyncEnumerable<string> StreamAnswerAsync(
        string query,
        IReadOnlyList<DocumentChunk> context,
        CancellationToken ct = default);
}
