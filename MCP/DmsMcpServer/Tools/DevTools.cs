using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace DmsMcpServer.Tools;

[McpServerToolType]
public class DevTools
{
    private readonly string _workspaceRoot;

    public DevTools(IConfiguration configuration)
    {
        _workspaceRoot = configuration["Workspace:Root"]
            ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
    }

    [McpServerTool(Name = "get_user_story")]
    [Description(
        "Belirtilen kullanıcı hikayesini (user story) docs/business/ klasöründen okur. " +
        "Kabul kriterleri, iş kuralları ve teknik notları içerir. " +
        "Kod üretimi veya test yazımı öncesinde bağlam oluşturmak için kullanın. " +
        "Mevcut user story numaraları: US-001 (Araç Ekleme), US-002 (Araç Listeleme), " +
        "US-003 (Opsiyonlama), US-004 (Müşteri Yönetimi), US-005 (Danışman Dashboard), " +
        "US-006 (Hızlı Müşteri Kaydı).")]
    public string GetUserStory(
        [Description("User story numarası, örn: US-001 veya 001 veya 1")] string number)
    {
        var fileName = ResolveFileName("business", "_us", number, "US");
        if (fileName is null)
            return $"User story bulunamadı: '{number}'. Mevcut: US-001 ile US-006 arası.";

        return ReadDocFile(fileName);
    }

    [McpServerTool(Name = "get_domain_entity")]
    [Description(
        "Domain entity veya value object tanımını docs/domain-model/ klasöründen okur. " +
        "Entity property'leri, iş kuralları ve factory method imzalarını içerir. " +
        "Mevcut entity'ler: vehicle, customer, service-advisor, vehicle-option. " +
        "Mevcut value object'ler: vin, money, email.")]
    public string GetDomainEntity(
        [Description("Entity veya value object adı: vehicle | customer | service-advisor | vehicle-option | vin | money | email")] string name)
    {
        var docsPath = Path.Combine(_workspaceRoot, "docs", "domain-model");

        if (!Directory.Exists(docsPath))
            return $"Domain model klasörü bulunamadı: {docsPath}";

        var searchName = name.Trim().ToLowerInvariant();
        var files = Directory.GetFiles(docsPath, "*.md")
            .Select(f => (Path: f, Name: Path.GetFileNameWithoutExtension(f).ToLowerInvariant()))
            .ToList();

        var match = files.FirstOrDefault(f =>
            f.Name == $"entity-{searchName}" ||
            f.Name == $"value-object-{searchName}" ||
            f.Name.EndsWith($"-{searchName}"));

        if (match.Path is null)
        {
            var available = files.Select(f => f.Name).ToList();
            return $"'{name}' için domain dosyası bulunamadı. Mevcut dosyalar: {string.Join(", ", available)}";
        }

        return ReadDocFile(match.Path);
    }

    [McpServerTool(Name = "list_adrs")]
    [Description(
        "Projedeki tüm Architecture Decision Record'ların (ADR) özetini listeler. " +
        "Her ADR için numara, başlık ve karar durumu gösterilir. " +
        "Belirli bir ADR'nin tam içeriğini okumak için get_adr tool'unu kullanın.")]
    public string ListAdrs()
    {
        var adrPath = Path.Combine(_workspaceRoot, "docs", "adr");

        if (!Directory.Exists(adrPath))
            return $"ADR klasörü bulunamadı: {adrPath}";

        var files = Directory.GetFiles(adrPath, "ADR-*.md")
            .OrderBy(f => f)
            .ToList();

        if (files.Count == 0)
            return "ADR klasöründe kayıt bulunamadı.";

        var sb = new StringBuilder();
        sb.AppendLine($"Toplam {files.Count} ADR:\n");

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            var title = ExtractMarkdownHeading(content) ?? Path.GetFileNameWithoutExtension(file);
            var status = ExtractAdrStatus(content) ?? "Bilinmiyor";
            var adrNumber = Path.GetFileNameWithoutExtension(file)[..7]; // ADR-001

            sb.AppendLine($"• {adrNumber}: {CleanTitle(title)} [{status}]");
        }

        return sb.ToString().TrimEnd();
    }

    [McpServerTool(Name = "get_adr")]
    [Description(
        "Belirtilen Architecture Decision Record'ın (ADR) tam içeriğini döner. " +
        "Mimari kararların gerekçesini ve değerlendirilen alternatifleri içerir. " +
        "Mevcut ADR'ler: ADR-001 (Clean Architecture), ADR-002 (CQRS/MediatR), " +
        "ADR-003 (DDD), ADR-004 (PostgreSQL), ADR-005 (EF Core+Dapper), " +
        "ADR-006 (Keycloak), ADR-007 (Vue3), ADR-008 (Serilog), " +
        "ADR-009 (FluentValidation), ADR-010 (API Versioning), " +
        "ADR-011 (xUnit), ADR-012 (GitHub Actions), ADR-013 (SonarQube).")]
    public string GetAdr(
        [Description("ADR numarası, örn: ADR-001 veya 001 veya 1")] string number)
    {
        var fileName = ResolveFileName("adr", "_adr", number, "ADR");
        if (fileName is null)
            return $"ADR bulunamadı: '{number}'. Mevcut: ADR-001 ile ADR-013 arası.";

        return ReadDocFile(fileName);
    }

    private string? ResolveFileName(string subFolder, string _, string number, string prefix)
    {
        var folder = Path.Combine(_workspaceRoot, "docs", subFolder);
        if (!Directory.Exists(folder))
            return null;

        number = number.Trim().TrimStart('0');
        if (!int.TryParse(Regex.Replace(number, @"\D", ""), out var n))
            return null;

        var padded = n.ToString("D3");

        var match = Directory
            .GetFiles(folder, $"{prefix}-{padded}*.md")
            .FirstOrDefault();

        return match;
    }

    private static string ReadDocFile(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            return $"Dosya okunamadı ({filePath}): {ex.Message}";
        }
    }

    private static string? ExtractMarkdownHeading(string content)
    {
        var line = content.Split('\n')
            .Select(l => l.Trim())
            .FirstOrDefault(l => l.StartsWith("# "));
        return line?[2..].Trim();
    }

    private static string? ExtractAdrStatus(string content)
    {
        var lines = content.Split('\n').Select(l => l.Trim()).ToList();
        for (int i = 0; i < lines.Count - 1; i++)
        {
            if (lines[i].Equals("## Durum", StringComparison.OrdinalIgnoreCase) ||
                lines[i].Equals("## Status", StringComparison.OrdinalIgnoreCase))
            {
                return lines.Skip(i + 1).FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));
            }
        }

        return null;
    }

    private static string CleanTitle(string title)
    {
        return Regex.Replace(title, @"^ADR-\d+:\s*", "").Trim();
    }
}
