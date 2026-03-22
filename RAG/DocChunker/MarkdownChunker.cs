using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DocChunker;

/// <summary>
/// Markdown dokümanlarını başlık hiyerarşisine göre semantik olarak parçalar.
/// <para>
/// Strateji:
/// 1. H1/H2/H3 başlıklarını sınır noktası olarak kullan.
/// 2. Her chunk için üst başlıkların tam breadcrumb'ını koru.
/// 3. Büyük bölümleri paragraf sınırlarında örtüşmeli (overlap) alt parçalara böl.
/// 4. Her chunk'a dosya + kategori + başlık yolu bağlamı ekle (EmbeddingText üzerinden).
/// </para>
/// </summary>
public static class MarkdownChunker
{
    private static readonly Regex HeadingRx =
        new(@"^(#{1,3})\s+(.+?)$", RegexOptions.Multiline | RegexOptions.Compiled);

    // Kod bloklarını atlamak için basit desen
    private static readonly Regex CodeBlockRx =
        new(@"```[\s\S]*?```", RegexOptions.Compiled);

    public static List<DocumentChunk> Chunk(
        string markdown, string sourcePath, string category,
        int maxChars, int minChars)
    {
        var chunks = new List<DocumentChunk>();

        // İlk H1'i doküman başlığı olarak al
        var titleMatch = HeadingRx.Matches(markdown)
            .Cast<Match>()
            .FirstOrDefault(m => m.Groups[1].Value == "#");
        var docTitle = titleMatch is not null
            ? titleMatch.Groups[2].Value.Trim()
            : Path.GetFileNameWithoutExtension(sourcePath);

        foreach (var (headingPath, content) in ExtractSections(markdown, docTitle))
        {
            var text = content.Trim();
            if (text.Length < minChars) continue;

            if (text.Length <= maxChars)
            {
                chunks.Add(Build(category, sourcePath, docTitle, headingPath, text));
            }
            else
            {
                // Büyük bölümü paragraf bazlı örtüşmeli parçalara böl
                foreach (var sub in SplitByParagraphs(text, maxChars, overlap: 200))
                {
                    if (sub.Length >= minChars)
                        chunks.Add(Build(category, sourcePath, docTitle, headingPath, sub));
                }
            }
        }

        return chunks;
    }

    private static IEnumerable<(string HeadingPath, string Content)> ExtractSections(
        string markdown, string docTitle)
    {
        var matches = HeadingRx.Matches(markdown).Cast<Match>().ToList();

        if (matches.Count == 0)
        {
            yield return (docTitle, markdown);
            yield break;
        }

        // İlk başlıktan önceki giriş metni
        if (matches[0].Index > 0)
        {
            var intro = markdown[..matches[0].Index].Trim();
            if (intro.Length > 0)
                yield return (docTitle, intro);
        }

        // Üst başlık yığını: indeks = seviye (1/2/3)
        var stack = new string?[4];

        for (int i = 0; i < matches.Count; i++)
        {
            var m = matches[i];
            var level = m.Groups[1].Value.Length;   // 1, 2 veya 3
            var title = m.Groups[2].Value.Trim();

            stack[level] = title;
            // Alt seviyeleri temizle
            for (int l = level + 1; l <= 3; l++) stack[l] = null;

            var contentStart = m.Index + m.Length;
            var contentEnd = i + 1 < matches.Count ? matches[i + 1].Index : markdown.Length;
            var content = markdown[contentStart..contentEnd].Trim();

            if (content.Length == 0) continue;

            // Breadcrumb: H1 > H2 > H3
            var headingPath = string.Join(" > ", stack[1..4].Where(s => s is not null));
            yield return (headingPath, content);
        }
    }

    private static IEnumerable<string> SplitByParagraphs(string text, int maxChars, int overlap)
    {
        // Çift satır sonu ile paragrafları ayır
        var paragraphs = Regex.Split(text, @"\n{2,}")
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        var sb = new StringBuilder();
        var prevTail = ""; // overlap için önceki chunk'ın son kısmı

        foreach (var para in paragraphs)
        {
            if (sb.Length + para.Length + 2 > maxChars && sb.Length > 0)
            {
                yield return sb.ToString().Trim();

                sb.Clear();
                if (prevTail.Length > 0)
                {
                    sb.AppendLine(prevTail);
                    sb.AppendLine();
                }
            }

            prevTail = para.Length > overlap ? para[^overlap..] : para;
            sb.AppendLine(para);
            sb.AppendLine();
        }

        if (sb.Length > 0)
            yield return sb.ToString().Trim();
    }

    private static DocumentChunk Build(
        string category, string source, string docTitle,
        string sectionPath, string content) => new()
        {
            Category = category,
            SourceFile = source,
            DocumentTitle = docTitle,
            SectionPath = sectionPath,
            Content = content,
        };
}
