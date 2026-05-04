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
                status = o.Status,
                isExpired = o.IsExpired,
                expiresAt = o.ExpiresAt.ToString("yyyy-MM-dd"),
                optionFee = $"{o.OptionFeeAmount} {o.OptionFeeCurrency}",
                notes = o.Notes
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}
