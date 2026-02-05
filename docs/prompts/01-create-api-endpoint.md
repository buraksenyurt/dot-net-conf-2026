# Prompt: Uçtan Uca API Endpoint Geliştirme

## Amaç
Bu prompt, GitHub Copilot'a belirli bir user story için backend API endpoint'ini uçtan uca (Command/Query, Handler, Controller, Tests) geliştirmesi için kullanılır.

## Prompt Template

```markdown
# User Story
[docs/business/US-XXX-xxx.md dosyasını buraya dahil et veya özetle]

# Domain Model
[docs/domain-model/ altındaki ilgili entity ve value object dokümanlarını referans et]

# Teknik Gereksinimler
- Clean Architecture prensiplerine uy
- CQRS pattern kullan
- MediatR ile handler'ları implement et
- FluentValidation ile validasyonları yaz
- Repository pattern kullan
- Unit testler yaz (xUnit, Moq, FluentAssertions)

# Dosya Yapısı
Aşağıdaki dosyaları oluştur:

## Application Layer
1. `Commands/[FeatureName]/[CommandName]Command.cs` - Command DTO
2. `Commands/[FeatureName]/[CommandName]CommandHandler.cs` - Command handler
3. `Commands/[FeatureName]/[CommandName]CommandValidator.cs` - FluentValidation validator
4. `Queries/[FeatureName]/[QueryName]Query.cs` - Query DTO
5. `Queries/[FeatureName]/[QueryName]QueryHandler.cs` - Query handler
6. `Queries/[FeatureName]/[ResponseName]Dto.cs` - Response DTO

## API Layer
7. `Controllers/[ControllerName]Controller.cs` - API Controller

## Tests
8. `[FeatureName]/[CommandName]CommandHandlerTests.cs` - Unit tests

# Kodlama Standartları
- docs/architectural-overview/02-coding-standards.md dosyasındaki standartlara uy
- Async/await kullan
- Exception handling için domain exception'lar kullan
- Loglama için ILogger kullan
- CancellationToken parametresi ekle

# Örnek İstek

US-001: Araç Envanterine Yeni Araç Ekleme için uçtan uca API endpoint geliştir.

Domain modelleri:
- Vehicle (Entity)
- VIN (Value Object)
- Money (Value Object)

İstenen:
1. AddVehicleCommand ve Handler
2. AddVehicleCommandValidator
3. VehiclesController'a POST endpoint
4. Unit testler

Lütfen tüm dosyaları kodlama standartlarına uygun şekilde üret.
```

## Kullanım

### Copilot Chat'te Kullanım
1. User story dosyasını aç
2. Domain model dosyalarını context'e ekle
3. Yukarıdaki prompt'u kopyala ve özelleştir
4. Copilot'a gönder

### Agent Kullanımı
```markdown
@agent-backend-developer Yukarıdaki user story için uçtan uca API endpoint geliştir. 
Context: #file:docs/business/US-001-add-vehicle-to-inventory.md 
         #file:docs/domain-model/entity-vehicle.md
         #file:docs/domain-model/value-object-vin.md
```

## Beklenen Çıktı Örneği

### AddVehicleCommand.cs
```csharp
namespace VehicleManagement.Application.Commands.AddVehicle;

public record AddVehicleCommand(
    string VIN,
    string Brand,
    string Model,
    int Year,
    string Color,
    EngineType EngineType,
    TransmissionType TransmissionType,
    decimal PurchaseAmount,
    Currency PurchaseCurrency,
    decimal SuggestedAmount,
    Currency SuggestedCurrency,
    int EngineCapacity,
    decimal FuelConsumption,
    int Mileage,
    List<string>? Features
) : IRequest<Guid>;
```

### AddVehicleCommandHandler.cs
```csharp
namespace VehicleManagement.Application.Commands.AddVehicle;

public class AddVehicleCommandHandler : IRequestHandler<AddVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<AddVehicleCommandHandler> _logger;

    public AddVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        ILogger<AddVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        // VIN oluştur
        var vin = new VIN(request.VIN);
        
        // Duplicate VIN kontrolü
        if (await _vehicleRepository.ExistsByVinAsync(vin, cancellationToken))
            throw new VehicleDomainException($"Bu VIN numarası zaten kayıtlı: {vin}");
        
        // Money value object'leri oluştur
        var purchasePrice = new Money(request.PurchaseAmount, request.PurchaseCurrency);
        var suggestedPrice = new Money(request.SuggestedAmount, request.SuggestedCurrency);
        
        // Vehicle entity oluştur
        var vehicle = new Vehicle(
            vin,
            request.Brand,
            request.Model,
            request.Year,
            request.Color,
            request.EngineType,
            request.TransmissionType,
            purchasePrice,
            dealerId: GetCurrentDealerId() // Context'ten al
        );
        
        // Additional properties
        vehicle.UpdatePrice(suggestedPrice);
        // ... diğer property'leri set et
        
        // Kaydet
        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        await _vehicleRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Yeni araç eklendi. ID: {VehicleId}, VIN: {VIN}", vehicle.Id, vin);
        
        return vehicle.Id;
    }
}
```

### VehiclesController.cs
```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddVehicle(
        [FromBody] AddVehicleCommand command,
        CancellationToken cancellationToken)
    {
        var vehicleId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetVehicle), new { id = vehicleId }, vehicleId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVehicle(Guid id)
    {
        // Implementation
        return Ok();
    }
}
```

## Notlar
- Prompt'u her user story'ye göre özelleştir
- Context olarak ilgili domain model dosyalarını ekle
- Kod üretildikten sonra manuel review yap
- Testleri çalıştır ve code coverage kontrol et
