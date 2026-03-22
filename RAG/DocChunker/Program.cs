using DocChunker;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;
using System.Text;
using System.Text.RegularExpressions;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .Build();

var settings = config.GetSection("DocChunker").Get<DocChunkerSettings>()
    ?? throw new InvalidOperationException("appsettings.json içinde 'DocChunker' bölümü bulunamadı.");

using var loggerFactory = LoggerFactory.Create(b => b
    .SetMinimumLevel(LogLevel.Information)
    .AddSimpleConsole(o =>
    {
        o.ColorBehavior = LoggerColorBehavior.Enabled;
        o.SingleLine = true;
        o.TimestampFormat = "HH:mm:ss ";
    }));
var logger = loggerFactory.CreateLogger("DocChunker");

var docsPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs"));

if (!Directory.Exists(docsPath))
{
    logger.LogError("Docs klasörü bulunamadı: {DocsPath}", docsPath);
    return 1;
}

logger.LogInformation("Docs klasörü: {DocsPath}", docsPath);

var openAiClient = new OpenAIClient(
    new ApiKeyCredential("lm-studio"),
    new OpenAIClientOptions { Endpoint = new Uri(settings.LmStudioUrl) });

#pragma warning disable SKEXP0010
var kernel = Kernel.CreateBuilder()
    .AddOpenAIEmbeddingGenerator(settings.EmbeddingModel, openAiClient)
    .Build();
#pragma warning restore SKEXP0010
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

var qdrant = new QdrantClient(settings.QdrantHost, settings.QdrantGrpcPort);
var vectorStore = new QdrantVectorStore(qdrant, ownsClient: true);
var collection = vectorStore.GetCollection<Guid, DocumentChunk>(settings.CollectionName);

logger.LogInformation("Servisler hazır. Koleksiyon oluşturuluyor...");
await collection.EnsureCollectionExistsAsync();
logger.LogInformation("Koleksiyon '{CollectionName}' hazır.", settings.CollectionName);

logger.LogInformation("Dokümanlar parçalanıyor...");
var allChunks = new List<DocumentChunk>();

foreach (var file in Directory.EnumerateFiles(docsPath, "*.md", SearchOption.AllDirectories))
{
    var relPath = Path.GetRelativePath(docsPath, file).Replace('\\', '/');
    var category = relPath.Contains('/') ? relPath.Split('/')[0] : "root";
    var markdown = await File.ReadAllTextAsync(file);
    var chunks = MarkdownChunker.Chunk(markdown, relPath, category, settings.MaxChunkChars, settings.MinChunkChars);
    allChunks.AddRange(chunks);
    logger.LogInformation("{RelPath,-55} \u2192 {ChunkCount,3} chunk", relPath, chunks.Count);
}

var fileCount = Directory.GetFiles(docsPath, "*.md", SearchOption.AllDirectories).Length;
logger.LogInformation("Toplam {ChunkCount} chunk ({FileCount} dosya).", allChunks.Count, fileCount);

logger.LogInformation("Embedding oluşturuluyor ve Qdrant'a yükleniyor...");
var total = allChunks.Count;

for (int i = 0; i < total; i += settings.BatchSize)
{
    var batch = allChunks.Skip(i).Take(settings.BatchSize).ToList();
    var inputTexts = batch.Select(c => c.EmbeddingText).ToList();

    GeneratedEmbeddings<Embedding<float>> embeddings;
    try
    {
        embeddings = await embeddingGenerator.GenerateAsync(inputTexts);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Embedding oluşturulamadı (batch {BatchStart}).", i);
        continue;
    }

    for (int j = 0; j < batch.Count; j++)
        batch[j].Embedding = embeddings[j].Vector;

    try
    {
        foreach (var chunk in batch)
            await collection.UpsertAsync(chunk);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Qdrant upsert başarısız (batch {BatchStart}).", i);
    }

    var done = Math.Min(i + settings.BatchSize, total);
    logger.LogInformation("{Done}/{Total} işlendi ({Percent:P0}).", done, total, (double)done / total);
}

logger.LogInformation("Tamamlandı! {Total} chunk '{CollectionName}' koleksiyonuna eklendi.",
    total, settings.CollectionName);
return 0;