using System.ComponentModel;
using System.Text.Json;
using DmsMcpServer.HttpClients;
using ModelContextProtocol.Server;

namespace DmsMcpServer.Tools;

[McpServerToolType]
public class CustomerTools
{
    private readonly DmsApiClient _api;

    public CustomerTools(DmsApiClient api) => _api = api;

    [McpServerTool(Name = "list_customers")]
    [Description(
        "DMS müşteri kayıtlarını listeler ve arama yapar. " +
        "search parametresi ile ad, soyad, e-posta veya şirket adında serbest metin araması yapılabilir. " +
        "customerType ile sadece bireysel (Individual) veya kurumsal (Corporate) müşteriler filtrelenebilir.")]
    public async Task<string> ListCustomers(
        [Description("Serbest metin araması: ad, soyad, e-posta veya şirket adı")] string? search = null,
        [Description("Müşteri tipi filtresi: Individual | Corporate")] string? customerType = null,
        [Description("Sayfa numarası (varsayılan: 1)")] int page = 1,
        [Description("Sayfa boyutu (varsayılan: 20)")] int pageSize = 20)
    {
        var result = await _api.GetCustomersAsync(search, customerType, page, pageSize);

        if (!result.Success)
            return $"Hata: {result.Error}";

        var paged = result.Data!;

        if (!paged.Items.Any())
            return string.IsNullOrWhiteSpace(search)
                ? "Sistemde kayıtlı müşteri bulunmuyor."
                : $"'{search}' araması için müşteri bulunamadı.";

        return JsonSerializer.Serialize(new
        {
            totalCount = paged.TotalCount,
            page = paged.Page,
            totalPages = paged.TotalPages,
            customers = paged.Items.Select(c => new
            {
                id = c.Id,
                displayName = c.DisplayName,
                email = c.Email,
                phone = c.Phone,
                customerType = c.CustomerType,
                companyName = c.CompanyName
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool(Name = "register_customer")]
    [Description(
        "Sisteme yeni bir müşteri kaydı oluşturur. " +
        "customerType için 'Individual' (bireysel) veya 'Corporate' (kurumsal) değeri girilmelidir. " +
        "Kurumsal müşteriler için companyName ve taxNumber zorunludur. " +
        "E-posta adresi sistemde benzersiz olmalıdır; aynı e-posta ile ikinci kayıt oluşturulamaz.")]
    public async Task<string> RegisterCustomer(
        [Description("Müşteri adı")] string firstName,
        [Description("Müşteri soyadı")] string lastName,
        [Description("E-posta adresi (sistem genelinde benzersiz olmalı)")] string email,
        [Description("Telefon numarası")] string phone,
        [Description("Müşteri tipi: Individual | Corporate")] string customerType,
        [Description("Şirket adı (kurumsal müşteriler için zorunlu)")] string? companyName = null,
        [Description("Vergi numarası (kurumsal müşteriler için zorunlu)")] string? taxNumber = null)
    {
        var request = new RegisterCustomerRequest(
            firstName, lastName, email, phone, customerType, companyName, taxNumber);

        var result = await _api.RegisterCustomerAsync(request);

        if (!result.Success)
        {
            if (result.Error?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true ||
                result.Error?.Contains("409", StringComparison.OrdinalIgnoreCase) == true)
                return $"Müşteri kaydedilemedi: '{email}' e-posta adresi zaten sistemde kayıtlı.";

            return $"Müşteri kaydedilemedi: {result.Error}";
        }

        var typeLabel = customerType.Equals("Corporate", StringComparison.OrdinalIgnoreCase)
            ? "Kurumsal"
            : "Bireysel";

        return $"Müşteri başarıyla kaydedildi. ID: {result.Data} | {firstName} {lastName} | {typeLabel}";
    }
}
