using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DmsMcpServer.HttpClients;

// ─── Response Models ───────────────────────────────────────────────────────────

public record VehicleResult(
    Guid Id,
    string VIN,
    string Brand,
    string Model,
    int Year,
    string EngineType,
    int Mileage,
    string Color,
    decimal PurchaseAmount,
    string PurchaseCurrency,
    decimal SuggestedAmount,
    string SuggestedCurrency,
    string TransmissionType,
    decimal FuelConsumption,
    int EngineCapacity,
    List<string> Features,
    string Status,
    DateTime CreatedAt
);

public record CustomerResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CustomerType,
    string? CompanyName,
    string? TaxNumber,
    string DisplayName,
    DateTime CreatedAt
);

public record VehicleOptionResult(
    Guid Id,
    Guid VehicleId,
    string VehicleDisplayName,
    string VehicleVIN,
    Guid CustomerId,
    string CustomerDisplayName,
    DateTime ExpiresAt,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    string? Notes,
    string Status,
    bool IsExpired,
    DateTime CreatedAt,
    Guid? ServiceAdvisorId,
    string? ServiceAdvisorDisplayName
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);

// ─── Request Models ────────────────────────────────────────────────────────────

public record AddVehicleRequest(
    string Vin,
    string Brand,
    string Model,
    int Year,
    string EngineType,
    int Mileage,
    string Color,
    decimal PurchaseAmount,
    string PurchaseCurrency,
    decimal SuggestedAmount,
    string SuggestedCurrency,
    string TransmissionType,
    decimal FuelConsumption,
    int EngineCapacity,
    List<string>? Features = null
);

public record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string CustomerType,
    string? CompanyName = null,
    string? TaxNumber = null
);

public record CreateOptionRequest(
    Guid VehicleId,
    Guid CustomerId,
    int ValidityDays,
    decimal OptionFeeAmount,
    string OptionFeeCurrency,
    Guid? ServiceAdvisorId = null,
    string? Notes = null
);

// ─── API Result Wrapper ────────────────────────────────────────────────────────

public record ApiResponse<T>(bool Success, T? Data, string? Error);

// ─── DmsApiClient ──────────────────────────────────────────────────────────────

/// <summary>
/// DMS backend API'ye ait tüm HTTP çağrılarını yöneten Typed HttpClient.
/// </summary>
public class DmsApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public DmsApiClient(HttpClient http)
    {
        _http = http;
    }

    // ── Vehicles ───────────────────────────────────────────────────────────────

    public async Task<ApiResponse<PagedResult<VehicleResult>>> GetVehiclesAsync(
        string? status = null,
        string? brand = null,
        string? model = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = BuildQuery(
            ("page", page.ToString()),
            ("pageSize", pageSize.ToString()),
            ("status", status),
            ("brand", brand)
        );

        return await GetAsync<PagedResult<VehicleResult>>($"api/v1/vehicles{query}");
    }

    public async Task<ApiResponse<Guid>> AddVehicleAsync(AddVehicleRequest request)
    {
        return await PostForIdAsync("api/v1/vehicles", request);
    }

    // ── Customers ──────────────────────────────────────────────────────────────

    public async Task<ApiResponse<PagedResult<CustomerResult>>> GetCustomersAsync(
        string? search = null,
        string? customerType = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = BuildQuery(
            ("page", page.ToString()),
            ("pageSize", pageSize.ToString()),
            ("search", search),
            ("customerType", customerType)
        );

        return await GetAsync<PagedResult<CustomerResult>>($"api/v1/customers{query}");
    }

    public async Task<ApiResponse<Guid>> RegisterCustomerAsync(RegisterCustomerRequest request)
    {
        return await PostForIdAsync("api/v1/customers", request);
    }

    // ── Vehicle Options ────────────────────────────────────────────────────────

    public async Task<ApiResponse<Guid>> CreateOptionAsync(CreateOptionRequest request)
    {
        return await PostForIdAsync("api/vehicle-options", request);
    }

    public async Task<ApiResponse<bool>> CancelOptionAsync(Guid optionId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/vehicle-options/{optionId}");

            if (response.IsSuccessStatusCode)
                return new ApiResponse<bool>(true, true, null);

            var body = await response.Content.ReadAsStringAsync();
            return new ApiResponse<bool>(false, false, ExtractError(body, response.StatusCode));
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>(false, false, ex.Message);
        }
    }

    public async Task<ApiResponse<List<VehicleOptionResult>>> GetAdvisorDashboardAsync(Guid advisorId)
    {
        return await GetAsync<List<VehicleOptionResult>>($"api/service-advisors/{advisorId}/dashboard");
    }

    // ── Private Helpers ────────────────────────────────────────────────────────

    private async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                return new ApiResponse<T>(true, data, null);
            }

            var body = await response.Content.ReadAsStringAsync();
            return new ApiResponse<T>(false, default, ExtractError(body, response.StatusCode));
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>(false, default, ex.Message);
        }
    }

    private async Task<ApiResponse<Guid>> PostForIdAsync<TRequest>(string url, TRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(url, request, JsonOptions);

            if (response.IsSuccessStatusCode)
            {
                // API { "id": "guid-string" } döner
                var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOptions);
                var id = body.GetProperty("id").GetGuid();
                return new ApiResponse<Guid>(true, id, null);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            return new ApiResponse<Guid>(false, Guid.Empty, ExtractError(errorBody, response.StatusCode));
        }
        catch (Exception ex)
        {
            return new ApiResponse<Guid>(false, Guid.Empty, ex.Message);
        }
    }

    private static string BuildQuery(params (string Key, string? Value)[] pairs)
    {
        var parts = pairs
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}");

        var qs = string.Join("&", parts);
        return string.IsNullOrEmpty(qs) ? string.Empty : $"?{qs}";
    }

    private static string ExtractError(string body, System.Net.HttpStatusCode statusCode)
    {
        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(body, JsonOptions);
            if (json.TryGetProperty("error", out var errorProp))
                return errorProp.GetString() ?? body;
        }
        catch { /* ignore parse errors */ }

        return $"HTTP {(int)statusCode} {statusCode}: {body}";
    }
}
