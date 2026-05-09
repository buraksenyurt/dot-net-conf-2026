using System.ComponentModel;
using System.Text.Json;
using DmsMcpServer.HttpClients;
using ModelContextProtocol.Server;

namespace DmsMcpServer.Tools;

[McpServerToolType]
public class OptionTools
{
    private readonly DmsApiClient _api;

    public OptionTools(DmsApiClient api) => _api = api;

    [McpServerTool(Name = "list_options")]
    [Description(
        "Araç opsiyonlarını listeler ve filtreler. " +
        "status parametresi ile durum filtrelemesi yapılabilir: " +
        "1=Active (Aktif), 2=Expired (Süresi Dolmuş), 3=Cancelled (İptal Edilmiş). " +
        "Parametre verilmezse tüm opsiyonlar listelenir. " +
        "customerSearch ile müşteri adına, vehicleSearch ile araç markası/modeli/VIN'e göre arama yapılabilir. " +
        "Örnek kullanım: süresi dolmuş opsiyonlar, aktif opsiyonlar, belirli müşterinin opsiyonları.")]
    public async Task<string> ListOptions(
        [Description("Durum filtresi: 1=Active, 2=Expired, 3=Cancelled. Boş bırakılırsa tümü listelenir.")] int? status = null,
        [Description("Müşteri adı ile arama (kısmi eşleşme)")] string? customerSearch = null,
        [Description("Araç markası, modeli veya VIN ile arama (kısmi eşleşme)")] string? vehicleSearch = null,
        [Description("Sayfa numarası (varsayılan: 1)")] int page = 1,
        [Description("Sayfa boyutu (varsayılan: 20, maks: 50)")] int pageSize = 20)
    {
        var result = await _api.GetOptionSummaryAsync(status, customerSearch, vehicleSearch, page, pageSize);

        if (!result.Success)
            return $"Hata: {result.Error}";

        var paged = result.Data!;

        if (!paged.Items.Any())
        {
            var filterDesc = status switch
            {
                1 => "aktif",
                2 => "süresi dolmuş",
                3 => "iptal edilmiş",
                _ => null
            };
            return filterDesc != null
                ? $"Kayıt bulunamadı: {filterDesc} opsiyon yok."
                : "Kayıt bulunamadı.";
        }

        var statusLabel = status switch
        {
            1 => "Aktif",
            2 => "Süresi Dolmuş",
            3 => "İptal Edilmiş",
            _ => "Tüm"
        };

        return JsonSerializer.Serialize(new
        {
            filtre = statusLabel,
            toplam = paged.TotalCount,
            sayfa = $"{paged.Page}/{paged.TotalPages}",
            opsiyonlar = paged.Items.Select(o => new
            {
                id = o.Id,
                araç = $"{o.VehicleDisplayName} ({o.VehicleVIN})",
                müşteri = o.CustomerDisplayName,
                durum = o.Status switch { 1 => "Active", 2 => "Expired", 3 => "Cancelled", _ => o.Status.ToString() },
                süresiDolmuMu = o.IsExpired,
                bitişTarihi = o.ExpiresAt.ToString("yyyy-MM-dd"),
                ücret = $"{o.OptionFeeAmount} {o.OptionFeeCurrency}",
                danışman = o.ServiceAdvisorDisplayName ?? "-",
                notlar = o.Notes
            })
        }, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
    }

    [McpServerTool(Name = "create_option")]
    [Description(
        "Bir aracı belirli bir müşteri adına rezerve eder (opsiyon oluşturur). " +
        "Opsiyon oluşturulduğunda araç durumu otomatik olarak 'Reserved' (Rezerve) statüsüne geçer. " +
        "Sadece 'InStock' (Stokta) veya 'OnSale' (Satışta) araçlar opsiyonlanabilir. " +
        "Halihazırda aktif opsiyonu olan veya 'Sold' (Satılmış) araçlar opsiyonlanamaz. " +
        "validityDays: 1-30 gün arası. optionFeeAmount: 0 veya daha büyük (ücretsiz opsiyon için 0 girilir).")]
    public async Task<string> CreateOption(
        [Description("Opsiyonlanacak aracın ID'si (GUID)")] Guid vehicleId,
        [Description("Müşterinin ID'si (GUID)")] Guid customerId,
        [Description("Opsiyon geçerlilik süresi gün cinsinden (1-30)")] int validityDays,
        [Description("Opsiyon ücreti / depozito tutarı (ücretsiz için 0)")] decimal optionFeeAmount,
        [Description("Opsiyon ücreti para birimi: TRY | USD | EUR")] string optionFeeCurrency,
        [Description("Opsiyonu oluşturan servis danışmanının ID'si (isteğe bağlı)")] Guid? serviceAdvisorId = null,
        [Description("Opsiyon notu (isteğe bağlı)")] string? notes = null)
    {
        if (validityDays < 1 || validityDays > 30)
            return "Hata: Opsiyon süresi 1 ile 30 gün arasında olmalıdır.";

        if (optionFeeAmount < 0)
            return "Hata: Opsiyon ücreti negatif olamaz.";

        var request = new CreateOptionRequest(
            vehicleId, customerId, validityDays,
            optionFeeAmount, optionFeeCurrency,
            serviceAdvisorId, notes);

        var result = await _api.CreateOptionAsync(request);

        if (!result.Success)
            return $"Opsiyon oluşturulamadı: {result.Error}";

        var expiresAt = DateTime.UtcNow.AddDays(validityDays).ToString("yyyy-MM-dd");

        return $"Opsiyon başarıyla oluşturuldu. " +
               $"ID: {result.Data} | " +
               $"Süre: {validityDays} gün (bitiş: {expiresAt}) | " +
               $"Araç durumu 'Reserved' (Rezerve) olarak güncellendi.";
    }

    [McpServerTool(Name = "cancel_option")]
    [Description(
        "Aktif bir araç opsiyonunu iptal eder. " +
        "İptal sonrasında araç durumu otomatik olarak 'OnSale' (Satışta) statüsüne geri döner. " +
        "Sadece 'Active' (Aktif) statüsündeki opsiyonlar iptal edilebilir.")]
    public async Task<string> CancelOption(
        [Description("İptal edilecek opsiyonun ID'si (GUID)")] Guid optionId)
    {
        var result = await _api.CancelOptionAsync(optionId);

        if (!result.Success)
        {
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return $"Opsiyon bulunamadı: ID '{optionId}'.";

            return $"Opsiyon iptal edilemedi: {result.Error}";
        }

        return $"Opsiyon başarıyla iptal edildi. ID: {optionId} | Araç durumu 'OnSale' (Satışta) olarak güncellendi.";
    }

    [McpServerTool(Name = "get_advisor_dashboard")]
    [Description(
        "Bir servis danışmanına atanmış tüm araç opsiyonlarını listeler. " +
        "Danışmanın güncel rezervasyon ve opsiyon durumlarını tek bakışta görmek için kullanılır. " +
        "Araç adı/VIN, müşteri adı, bitiş tarihi, opsiyon ücreti ve durum bilgisini içerir.")]
    public async Task<string> GetAdvisorDashboard(
        [Description("Servis danışmanının ID'si (GUID)")] Guid advisorId)
    {
        var result = await _api.GetAdvisorDashboardAsync(advisorId);

        if (!result.Success)
        {
            if (result.Error?.Contains("404", StringComparison.OrdinalIgnoreCase) == true)
                return $"Servis danışmanı bulunamadı: ID '{advisorId}'.";

            return $"Hata: {result.Error}";
        }

        var options = result.Data!;

        if (options.Count == 0)
            return $"Danışman ID '{advisorId}' için aktif opsiyon bulunmuyor.";

        return JsonSerializer.Serialize(new
        {
            advisorId,
            totalOptions = options.Count,
            options = options.Select(o => new
            {
                id = o.Id,
                vehicle = $"{o.VehicleDisplayName} ({o.VehicleVIN})",
                customer = o.CustomerDisplayName,
                status = o.Status switch { 1 => "Active", 2 => "Expired", 3 => "Cancelled", _ => o.Status.ToString() },
                isExpired = o.IsExpired,
                expiresAt = o.ExpiresAt.ToString("yyyy-MM-dd"),
                optionFee = $"{o.OptionFeeAmount} {o.OptionFeeCurrency}",
                notes = o.Notes
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}
