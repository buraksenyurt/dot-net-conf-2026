using System.ComponentModel;
using System.Text.Json;
using DmsMcpServer.HttpClients;
using ModelContextProtocol.Server;

namespace DmsMcpServer.Tools;

[McpServerToolType]
public class VehicleTools
{
    private readonly DmsApiClient _api;

    public VehicleTools(DmsApiClient api) => _api = api;

    [McpServerTool(Name = "list_vehicles")]
    [Description(
        "DMS araç envanterini listeler. " +
        "status parametresi ile araç durumuna göre filtre uygulanabilir: " +
        "InStock (Stokta), OnSale (Satışta), Reserved (Rezerve), Sold (Satıldı). " +
        "brand ve model ile marka/model filtrelemesi de yapılabilir.")]
    public async Task<string> ListVehicles(
        [Description("Araç durum filtresi: InStock | OnSale | Reserved | Sold")] string? status = null,
        [Description("Marka filtresi (örn: Honda, Toyota)")] string? brand = null,
        [Description("Model filtresi (örn: Civic, Corolla)")] string? model = null,
        [Description("Sayfa numarası (varsayılan: 1)")] int page = 1,
        [Description("Sayfa boyutu (varsayılan: 20)")] int pageSize = 20)
    {
        var result = await _api.GetVehiclesAsync(status, brand, model, page, pageSize);

        if (!result.Success)
            return $"Hata: {result.Error}";

        var paged = result.Data!;

        if (!paged.Items.Any())
            return "Belirtilen kriterlere uygun araç bulunamadı.";

        return JsonSerializer.Serialize(new
        {
            totalCount = paged.TotalCount,
            page = paged.Page,
            totalPages = paged.TotalPages,
            vehicles = paged.Items.Select(v => new
            {
                id = v.Id,
                vin = v.VIN,
                display = $"{v.Brand} {v.Model} {v.Year}",
                brand = v.Brand,
                model = v.Model,
                year = v.Year,
                color = v.Color,
                status = v.Status,
                engineType = v.EngineType,
                mileage = v.Mileage,
                suggestedPrice = $"{v.SuggestedAmount} {v.SuggestedCurrency}"
            })
        }, new JsonSerializerOptions { WriteIndented = true });
    }

    [McpServerTool(Name = "add_vehicle")]
    [Description(
        "Envantere yeni bir araç ekler. " +
        "Araç eklendikten sonra durumu otomatik olarak 'InStock' (Stokta) olarak ayarlanır. " +
        "VIN 17 karakter olmalı, I/O/Q harfleri içermemelidir. " +
        "EngineType: Gasoline | Diesel | Electric | Hybrid. " +
        "TransmissionType: Manual | Automatic | SemiAutomatic. " +
        "Currency: TRY | USD | EUR.")]
    public async Task<string> AddVehicle(
        [Description("VIN numarası — 17 karakter, ISO 3779")] string vin,
        [Description("Araç markası (örn: Honda)")] string brand,
        [Description("Araç modeli (örn: Civic)")] string model,
        [Description("Model yılı (örn: 2024)")] int year,
        [Description("Motor tipi: Gasoline | Diesel | Electric | Hybrid")] string engineType,
        [Description("Başlangıç kilometresi (yeni araç için 0)")] int mileage,
        [Description("Araç rengi")] string color,
        [Description("Alış fiyatı tutarı")] decimal purchaseAmount,
        [Description("Alış para birimi: TRY | USD | EUR")] string purchaseCurrency,
        [Description("Tavsiye edilen satış fiyatı")] decimal suggestedAmount,
        [Description("Satış fiyatı para birimi: TRY | USD | EUR")] string suggestedCurrency,
        [Description("Vites tipi: Manual | Automatic | SemiAutomatic")] string transmissionType,
        [Description("Yakıt tüketimi (L/100km; elektrikli araçlar için 0)")] decimal fuelConsumption,
        [Description("Motor hacmi cc (elektrikli için 0)")] int engineCapacity,
        [Description("Opsiyonel donatım listesi (örn: [\"Sunroof\",\"Deri Döşeme\"])")] List<string>? features = null)
    {
        var vinCheck = VinValidator.Validate(vin);
        if (!vinCheck.Valid)
            return $"Geçersiz VIN: {vinCheck.Message}";

        var request = new AddVehicleRequest(
            vin, brand, model, year, engineType, mileage, color,
            purchaseAmount, purchaseCurrency,
            suggestedAmount, suggestedCurrency,
            transmissionType, fuelConsumption, engineCapacity, features);

        var result = await _api.AddVehicleAsync(request);

        if (!result.Success)
            return $"Araç eklenemedi: {result.Error}";

        return $"Araç başarıyla eklendi. ID: {result.Data} | VIN: {vin.ToUpper()} | {brand} {model} {year}";
    }

    [McpServerTool(Name = "validate_vin")]
    [Description(
        "Bir VIN numarasını ISO 3779 standardına göre lokal olarak doğrular. " +
        "API çağrısı yapmaz; sadece format, karakter ve check digit kontrolü yapar. " +
        "Araç eklemeden önce VIN doğruluğunu kontrol etmek için kullanın.")]
    public string ValidateVin(
        [Description("Doğrulanacak VIN numarası (17 karakter)")] string vin)
    {
        var result = VinValidator.Validate(vin);
        return result.Valid
            ? $"✓ Geçerli VIN: '{vin.Trim().ToUpperInvariant()}'"
            : $"✗ Geçersiz VIN: {result.Message}";
    }
}

internal static class VinValidator
{
    private static readonly Dictionary<char, int> Transliterate = new()
    {
        ['A'] = 1, ['B'] = 2, ['C'] = 3, ['D'] = 4, ['E'] = 5, ['F'] = 6, ['G'] = 7, ['H'] = 8,
        ['J'] = 1, ['K'] = 2, ['L'] = 3, ['M'] = 4, ['N'] = 5,
        ['P'] = 7, ['R'] = 9,
        ['S'] = 2, ['T'] = 3, ['U'] = 4, ['V'] = 5, ['W'] = 6, ['X'] = 7, ['Y'] = 8, ['Z'] = 9,
        ['0'] = 0, ['1'] = 1, ['2'] = 2, ['3'] = 3, ['4'] = 4,
        ['5'] = 5, ['6'] = 6, ['7'] = 7, ['8'] = 8, ['9'] = 9
    };

    private static readonly int[] Weights = { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };

    public static (bool Valid, string Message) Validate(string vin)
    {
        vin = vin.Trim().ToUpperInvariant();

        if (vin.Length != 17)
            return (false, $"VIN tam 17 karakter olmalıdır (girilen: {vin.Length}).");

        if (vin.Any(c => c is 'I' or 'O' or 'Q'))
            return (false, "VIN içinde I, O veya Q harfi bulunamaz.");

        if (vin.Any(c => !Transliterate.ContainsKey(c)))
            return (false, "VIN geçersiz karakter içeriyor.");

        var sum = vin.Select((c, i) => Transliterate[c] * Weights[i]).Sum();
        var checkValue = sum % 11;
        var expectedChar = checkValue == 10 ? 'X' : (char)('0' + checkValue);

        if (vin[8] != expectedChar)
            return (false, $"Check digit hatalı (pozisyon 9). Beklenen: '{expectedChar}', Girilen: '{vin[8]}'.");

        return (true, "VIN geçerli.");
    }
}
