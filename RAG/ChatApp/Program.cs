using System.ClientModel;
using System.Text.Json;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Settings;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var settings = builder.Configuration.GetSection("ChatApp").Get<ChatAppSettings>()
    ?? throw new InvalidOperationException("appsettings.json içinde 'ChatApp' bölümü bulunamadı.");

builder.Services.Configure<ChatAppSettings>(builder.Configuration.GetSection("ChatApp"));

var openAiClient = new OpenAIClient(
    new ApiKeyCredential("lm-studio"),
    new OpenAIClientOptions { Endpoint = new Uri(settings.LmStudioUrl) });

builder.Services.AddSingleton(sp =>
    Kernel.CreateBuilder()
        .AddOpenAIEmbeddingGenerator(settings.EmbeddingModel, openAiClient)
        .AddOpenAIChatCompletion(settings.ChatModel, openAiClient)
        .Build());

builder.Services.AddSingleton<VectorStoreCollection<Guid, DocumentChunk>>(sp =>
{
    var qdrantClient = new QdrantClient(settings.QdrantHost, settings.QdrantGrpcPort);
    var vectorStore = new QdrantVectorStore(qdrantClient, ownsClient: true);
    return vectorStore.GetCollection<Guid, DocumentChunk>(settings.CollectionName);
});

builder.Services.AddSingleton<IRagChatService, RagChatService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.MapGet("/api/chat/stream", async (
    string q,
    IRagChatService ragService,
    HttpContext ctx,
    CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(q))
    {
        ctx.Response.StatusCode = 400;
        return;
    }

    ctx.Response.Headers.Append("Content-Type", "text/event-stream; charset=utf-8");
    ctx.Response.Headers.Append("Cache-Control", "no-cache");
    ctx.Response.Headers.Append("X-Accel-Buffering", "no");

    async Task WriteEventAsync(object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonSerializerOptions.Web);
        await ctx.Response.WriteAsync($"data: {json}\n\n", ct);
        await ctx.Response.Body.FlushAsync(ct);
    }

    try
    {
        var chunks = await ragService.SearchAsync(q, ct);

        await foreach (var token in ragService.StreamAnswerAsync(q, chunks, ct))
        {
            await WriteEventAsync(new { type = "token", content = token });
        }

        var sources = chunks.Select(c => new
        {
            file = c.SourceFile,
            section = c.SectionPath,
            category = c.Category
        });

        await WriteEventAsync(new { type = "sources", items = sources });
        await WriteEventAsync(new { type = "done" });
    }
    catch (OperationCanceledException)
    {
        // client disconnected — normal
    }
    catch (Exception ex)
    {
        await WriteEventAsync(new { type = "error", message = ex.Message });
    }
});

app.Run();
