---
name: create-api-endpoint
description: Clean Architecture prensipleri ile uçtan uca backend API endpoint geliştirme. CQRS Command/Query, MediatR handler, FluentValidation, API Controller ve xUnit unit testleri oluşturur. docs/business/US-XXX ve docs/domain-model/ dosyalarını context olarak gerektirir.
---

# Skill: Uçtan Uca API Endpoint Geliştirme

## Ne Zaman Kullanılır
- Yeni bir user story için backend implementasyonu başlatılacağında
- CQRS pattern ile yeni bir Command veya Query eklenecekse
- Controller endpoint + handler + validator + test dosyaları birlikte oluşturulacaksa

## Gerekli Context Dosyaları
- `docs/business/US-XXX-xxx.md` — User Story ve Acceptance Criteria
- `docs/domain-model/entity-*.md` — İlgili Entity tanımları
- `docs/domain-model/value-object-*.md` — İlgili Value Object tanımları
- `docs/architectural-overview/02-coding-standards.md` — Kodlama standartları

## Adım Adım Geliştirme Süreci

### 1. User Story ve Domain Analizi
- User story'yi oku ve kabul kriterlerini anla
- İlgili domain entity ve value object'leri belirle
- Command mi, Query mi, yoksa her ikisi birden mi gerektiğini karar ver

### 2. Application Layer Dosyaları

**Command senaryosu için:**
```
src/backend/VehicleInventory.Application/
├── Commands/
│   └── [FeatureName]/
│       ├── [CommandName]Command.cs           # IRequest<Guid> record
│       ├── [CommandName]CommandHandler.cs    # IRequestHandler<,> impl
│       └── [CommandName]CommandValidator.cs  # AbstractValidator<> impl
```

**Query senaryosu için:**
```
src/backend/VehicleInventory.Application/
├── Queries/
│   └── [FeatureName]/
│       ├── [QueryName]Query.cs               # IRequest<List<Dto>> record
│       ├── [QueryName]QueryHandler.cs        # IRequestHandler<,> impl
│       └── [ResponseName]Dto.cs              # Response DTO record
```

### 3. API Layer

```
src/backend/VehicleInventory.API/
└── Controllers/
    └── [ControllerName]Controller.cs
```

### 4. Unit Tests

```
tests/VehicleInventory.Application.Tests/
├── Commands/
│   └── [CommandName]CommandHandlerTests.cs
└── Queries/
    └── [QueryName]QueryHandlerTests.cs
```

## Kodlama Kuralları

### Command Record
```csharp
namespace VehicleInventory.Application.Commands.[FeatureName];

public record [CommandName]Command(
    // Properties buraya
) : IRequest<Guid>;
```

### Command Handler
```csharp
public class [CommandName]CommandHandler : IRequestHandler<[CommandName]Command, Guid>
{
    private readonly I[Entity]Repository _repository;
    private readonly ILogger<[CommandName]CommandHandler> _logger;

    public [CommandName]CommandHandler(I[Entity]Repository repository,
        ILogger<[CommandName]CommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> Handle([CommandName]Command request, CancellationToken cancellationToken)
    {
        // 1. Domain entity oluştur
        // 2. Repository'e kaydet
        // 3. SaveChangesAsync çağır
        // 4. Loglama yap
        // 5. ID döndür
    }
}
```

### FluentValidation Validator
```csharp
public class [CommandName]CommandValidator : AbstractValidator<[CommandName]Command>
{
    public [CommandName]CommandValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty()
            .MaximumLength(100);
    }
}
```

### Controller Action
```csharp
[HttpPost]
[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> Create([FromBody] [CommandName]Command command,
    CancellationToken cancellationToken)
{
    var id = await _mediator.Send(command, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id }, id);
}
```

### Unit Test
```csharp
public class [CommandName]CommandHandlerTests
{
    private readonly Mock<I[Entity]Repository> _repositoryMock = new();
    private readonly Mock<ILogger<[CommandName]CommandHandler>> _loggerMock = new();
    private readonly [CommandName]CommandHandler _handler;

    public [CommandName]CommandHandlerTests()
    {
        _handler = new [CommandName]CommandHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturn[Entity]Id()
    {
        // Arrange
        var command = new [CommandName]Command(/* valid params */);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<[Entity]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<[Entity]>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## Kontrol Listesi
- [ ] Command/Query record tanımlandı
- [ ] Handler async/await kullanıyor
- [ ] CancellationToken tüm async çağrılara iletildi
- [ ] FluentValidation kuralları yazıldı
- [ ] Controller `CreatedAtAction` veya `Ok` döndürüyor
- [ ] Unit testler `Arrange/Act/Assert` yapısında
- [ ] Exception'lar domain exception kullanıyor
- [ ] ILogger ile loglama yapıldı
