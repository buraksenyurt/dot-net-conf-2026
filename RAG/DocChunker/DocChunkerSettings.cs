namespace DocChunker;

/// <summary>
/// appsettings.json → "DocChunker" bölümüne karşılık gelen yapılandırma modeli.
/// </summary>
public sealed class DocChunkerSettings
{
    /// <summary>LM Studio OpenAI-uyumlu endpoint URL'i.</summary>
    public string LmStudioUrl { get; set; } = "http://localhost:1234/v1";

    /// <summary>Embedding model adı.</summary>
    public string EmbeddingModel { get; set; } = "text-embedding-nomic-embed-text-v1.5";

    /// <summary>Qdrant sunucu adresi.</summary>
    public string QdrantHost { get; set; } = "localhost";

    /// <summary>Qdrant gRPC portu.</summary>
    public int QdrantGrpcPort { get; set; } = 6334;

    /// <summary>Qdrant koleksiyon adı.</summary>
    public string CollectionName { get; set; } = "docs_knowledge_base";

    /// <summary>Bir chunk'ın maksimum karakter sayısı.</summary>
    public int MaxChunkChars { get; set; } = 1500;

    /// <summary>Bir chunk'ın minimum karakter sayısı (daha kısa chunk'lar atlanır).</summary>
    public int MinChunkChars { get; set; } = 60;

    /// <summary>Embedding batch boyutu.</summary>
    public int BatchSize { get; set; } = 8;
}
