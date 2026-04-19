---
name: run-and-analyze-tests
description: Backend (xUnit, dotnet test) ve frontend (Vitest, yarn test) testlerini çalıştırma, test hatalarını analiz etme ve düzeltme önerileri sunma. Test coverage raporunu değerlendirerek eksik coverage alanlarını belirler ve yeni test senaryoları önerir.
---

# Skill: Test Çalıştırma ve Analiz

## Ne Zaman Kullanılır
- Testlerin neden başarısız olduğu araştırılacaksa
- Test coverage artırılacaksa
- Yeni bir özellik için test senaryoları yazılacaksa
- CI/CD'de kırmızı olan testler düzeltilecekse

## Test Projesi Yapısı

```
tests/
├── VehicleInventory.Application.Tests/
│   ├── Commands/      # Command handler unit testleri
│   ├── Queries/       # Query handler unit testleri
│   └── Validators/    # FluentValidation testleri
└── VehicleInventory.Domain.Tests/
    └── (Domain entity ve value object testleri)

src/frontend/src/
└── __tests__/         # Vitest component testleri
```

## Backend Testleri Çalıştırma

### Tüm Testler
```bash
dotnet test tests/ --verbosity normal
```

### Belirli Test Projesi
```bash
dotnet test tests/VehicleInventory.Application.Tests --verbosity normal
```

### Coverage Raporu
```bash
dotnet test tests/ --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Test Filtresi
```bash
# Sadece belirli bir test sınıfı
dotnet test --filter "FullyQualifiedName~AddVehicleCommandHandlerTests"

# Belirli bir kategori
dotnet test --filter "Category=Unit"
```

## Frontend Testleri Çalıştırma

```bash
cd src/frontend
yarn test           # Tüm testler
yarn test --run     # Watch modunu kapatarak tek sefer çalıştır
yarn coverage       # Coverage raporu ile
```

## Test Hata Analizi

### Yaygın Backend Hataları

**1. NullReferenceException** — Mock setup eksik
```csharp
// Hata: Mock setup yapılmamış
// Çözüm:
_repositoryMock.Setup(r => r.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
    .Returns(Task.CompletedTask);
```

**2. Validation Hatası** — FluentValidation test data yanlış
```csharp
// Validator test yaparken geçersiz data kullan
var result = await _validator.ValidateAsync(new AddVehicleCommand("", ...));
result.IsValid.Should().BeFalse();
result.Errors.Should().Contain(e => e.PropertyName == "VIN");
```

**3. Repository SaveChanges çağrılmadı**
```csharp
// SaveChangesAsync mock'lanmalı
_unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
    .ReturnsAsync(1);
// Ve verify edilmeli
_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
```

## Unit Test Yazma Rehberi

### Test Class Yapısı (xUnit)
```csharp
public class [HandlerName]Tests
{
    // Mock'lar — her test için sıfırlanır (new Mock<>() constructor'da)
    private readonly Mock<I[Repository]> _repoMock = new();
    private readonly Mock<ILogger<[Handler]>> _loggerMock = new();
    private readonly [Handler] _handler;

    public [HandlerName]Tests()
    {
        _handler = new [Handler](_repoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_[Senaryo]_Should[Beklenti]()
    {
        // Arrange — test verisi ve mock setup
        // Act — metod çağrısı
        // Assert — sonuç doğrulama (FluentAssertions)
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("INVALID")]
    public async Task Handle_InvalidVIN_ShouldThrowException(string invalidVin)
    {
        // Parameterized test
    }
}
```

### Validator Testleri
```csharp
public class [Command]ValidatorTests
{
    private readonly [Command]Validator _validator = new();

    [Fact]
    public async Task Validate_ValidCommand_ShouldPassValidation()
    {
        var command = new [Command](/* valid data */);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyField_ShouldFailValidation()
    {
        var command = new [Command](/* invalid: empty field */);
        var result = await _validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FieldName");
    }
}
```

## Coverage Hedefleri

| Katman | Hedef Coverage |
|--------|---------------|
| Domain (Entities, Value Objects) | %90+ |
| Application (Handlers, Validators) | %80+ |
| Infrastructure | %60+ |

## Kontrol Listesi
- [ ] `dotnet test` çalıştırıldı, tüm testler yeşil
- [ ] Başarısız testler analiz edildi (hata mesajı okundu)
- [ ] Mock setup'lar eksik değil
- [ ] `Verify()` ile önemli çağrılar doğrulandı
- [ ] FluentAssertions ile anlamlı assertion'lar yazıldı
- [ ] Her test bağımsız çalışıyor (test sırası önemli değil)
- [ ] Parameterized testler (`[Theory]`) edge case'leri kapsıyor
