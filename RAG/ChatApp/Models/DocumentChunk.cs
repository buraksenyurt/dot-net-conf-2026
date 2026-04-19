using Microsoft.Extensions.VectorData;

namespace ChatApp.Models;

public sealed class DocumentChunk
{
    [VectorStoreKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    [VectorStoreData(IsIndexed = true)]
    public string Category { get; set; } = "";

    [VectorStoreData(IsIndexed = true)]
    public string SourceFile { get; set; } = "";

    [VectorStoreData]
    public string DocumentTitle { get; set; } = "";

    [VectorStoreData]
    public string SectionPath { get; set; } = "";

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Content { get; set; } = "";

    [VectorStoreVector(768, DistanceFunction = "CosineSimilarity")]
    public ReadOnlyMemory<float> Embedding { get; set; }

    public string EmbeddingText =>
        $"Kaynak: {SourceFile} | Kategori: {Category}\n" +
        $"Doküman: {DocumentTitle} > {SectionPath}\n\n" +
        Content;
}
