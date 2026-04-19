using System.Runtime.CompilerServices;
using System.Text;
using ChatApp.Models;
using ChatApp.Settings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;namespace ChatApp.Services;

public sealed class RagChatService : IRagChatService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly VectorStoreCollection<Guid, DocumentChunk> _collection;
    private readonly IChatCompletionService _chatService;
    private readonly int _topK;

    public RagChatService(
        Kernel kernel,
        VectorStoreCollection<Guid, DocumentChunk> collection,
        IOptions<ChatAppSettings> options)
    {
        _embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
        _collection = collection;
        _topK = options.Value.TopK;
    }

    public async Task<List<DocumentChunk>> SearchAsync(string query, CancellationToken ct = default)
    {
        var embeddings = await _embeddingGenerator.GenerateAsync([query], cancellationToken: ct);
        var queryVector = embeddings[0].Vector;

        var chunks = new List<DocumentChunk>();

        await foreach (var result in _collection.SearchAsync(queryVector, _topK, cancellationToken: ct))
        {
            chunks.Add(result.Record);
        }

        return chunks;
    }

    public async IAsyncEnumerable<string> StreamAnswerAsync(
        string query,
        IReadOnlyList<DocumentChunk> context,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var contextText = new StringBuilder();
        for (int i = 0; i < context.Count; i++)
        {
            var chunk = context[i];
            contextText.AppendLine($"--- Kaynak {i + 1}: {chunk.SourceFile} | {chunk.SectionPath} ---");
            contextText.AppendLine(chunk.Content);
            contextText.AppendLine();
        }

        var systemPrompt =
            "Sen bir yazılım geliştirici asistanısın. Aşağıdaki kaynak belgelerden yararlanarak soruya " +
            "Türkçe yanıt ver. Eğer bağlamda yeterli bilgi yoksa bunu açıkça belirt, bilgi uydurmaya çalışma.\n\n" +
            "BAĞLAM:\n" + contextText;

        var history = new ChatHistory(systemPrompt);
        history.AddUserMessage(query);

        await foreach (var content in _chatService
            .GetStreamingChatMessageContentsAsync(history, cancellationToken: ct)
            .WithCancellation(ct))
        {
            if (!string.IsNullOrEmpty(content.Content))
                yield return content.Content;
        }
    }
}
