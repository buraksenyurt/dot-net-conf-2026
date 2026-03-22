using Microsoft.Extensions.VectorData;

namespace DocChunker;

/// <summary>
/// Qdrant'ta saklanacak belge parçasını temsil eder.
/// Her property SK VectorStore eşlemesine göre işaretlenmiştir.
/// </summary>
public sealed class DocumentChunk
{
    [VectorStoreKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>docs/ altındaki klasör adı: adr, business, domain-model vb.</summary>
    [VectorStoreData(IsIndexed = true)]
    public string Category { get; set; } = "";

    /// <summary>docs/ içindeki göreli dosya yolu.</summary>
    [VectorStoreData(IsIndexed = true)]
    public string SourceFile { get; set; } = "";

    /// <summary>Dokümanın ilk H1 başlığı.</summary>
    [VectorStoreData]
    public string DocumentTitle { get; set; } = "";

    /// <summary>Bölüm hiyerarşisi: "H1 Başlık > H2 Başlık > H3 Başlık".</summary>
    [VectorStoreData]
    public string SectionPath { get; set; } = "";

    /// <summary>Bölümün ham metni.</summary>
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Content { get; set; } = "";

    /// <summary>768 boyutlu embedding vektörü (nomic-embed-text-v1.5).</summary>
    [VectorStoreVector(768, DistanceFunction = "CosineSimilarity")]
    public ReadOnlyMemory<float> Embedding { get; set; }

    /// <summary>
    /// Embedding oluşturmak için kullanılan bağlam-zenginleştirilmiş metin.
    /// Qdrant'a kaydedilmez; yalnızca runtime'da kullanılır.
    /// </summary>
    public string EmbeddingText =>
        $"Kaynak: {SourceFile} | Kategori: {Category}\n" +
        $"Doküman: {DocumentTitle} > {SectionPath}\n\n" +
        Content;
}
