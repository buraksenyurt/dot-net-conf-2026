namespace ChatApp.Settings;

public sealed class ChatAppSettings
{
    public string LmStudioUrl { get; set; } = "http://localhost:1234/v1";
    public string EmbeddingModel { get; set; } = "text-embedding-nomic-embed-text-v1.5";
    public string ChatModel { get; set; } = "meta-llama-3-8b-instruct";
    public string QdrantHost { get; set; } = "localhost";
    public int QdrantGrpcPort { get; set; } = 6334;
    public string CollectionName { get; set; } = "docs_knowledge_base";
    public int TopK { get; set; } = 5;
}
