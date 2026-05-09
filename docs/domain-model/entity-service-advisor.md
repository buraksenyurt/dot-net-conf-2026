# Domain Model: ServiceAdvisor (Servis Danışmanı)

## Entity Tanımı

**ServiceAdvisor** — Sistemde oturum açabilen ve müşteri adına araç opsiyonlarını yöneten çalışanı temsil eden entity.

## Sorumluluklar
- Servis danışmanı kimlik bilgilerini saklamak
- Oturum açma ve kimlik doğrulama için veri sağlamak
- Danışmana ait aktif ve geçmiş opsiyonları listelemek

## Properties

| Alan           | Tip              | Açıklama                                          |
|----------------|------------------|---------------------------------------------------|
| `Id`           | `Guid`           | Benzersiz tanımlayıcı (Primary Key)               |
| `FirstName`    | `string`         | Ad                                                |
| `LastName`     | `string`         | Soyad                                             |
| `Email`        | `Email`          | E-posta adresi (Value Object, unique)             |
| `PasswordHash` | `string`         | Bcrypt ile hashlenmiş parola                      |
| `Department`   | `string`         | Bölüm / Departman adı                            |
| `IsActive`     | `bool`           | Aktif mi? (varsayılan: `true`)                    |
| `CreatedAt`    | `DateTime`       | Oluşturulma tarihi (UTC)                          |

## Factory Method

```csharp
public static Result<ServiceAdvisor> Create(
    string firstName,
    string lastName,
    Email email,
    string passwordHash,
    string department) → Result<ServiceAdvisor>
```

**Validasyonlar:**
- `firstName`, `lastName`, `department`: boş olamaz
- `passwordHash`: boş olamaz (hash işlemi Application katmanında `IPasswordHasher` ile yapılır)

## Domain Methods

```csharp
// Danışmanı pasife çeker
public void Deactivate()

// Görünen adı döner
public string GetDisplayName() // "FirstName LastName"
```

## Kimlik Doğrulama Akışı

Parola doğrulaması domain'de değil, Application katmanında `IPasswordHasher.Verify(password, storedHash)` üzerinden yapılır. `ServiceAdvisor` yalnızca `PasswordHash` değerini saklar.

## İlişkiler

- **One-to-Many** with `VehicleOption` — bir danışman birden fazla opsiyon oluşturabilir (nullable FK)

## Indexes

```
- PK: Id
- UNIQUE: Email
- INDEX: IsActive
```

## Veritabanı Tablosu: `ServiceAdvisors`

| Kolon          | Tip           | Kısıtlamalar              |
|----------------|---------------|---------------------------|
| `Id`           | `uuid`        | PK                        |
| `FirstName`    | `varchar(100)` | NOT NULL                 |
| `LastName`     | `varchar(100)` | NOT NULL                 |
| `EmailValue`   | `varchar(254)` | NOT NULL, UNIQUE         |
| `PasswordHash` | `varchar(500)` | NOT NULL                 |
| `Department`   | `varchar(200)` | NOT NULL                 |
| `IsActive`     | `boolean`     | NOT NULL, DEFAULT true    |
| `CreatedAt`    | `timestamp`   | NOT NULL                  |

## API Endpoint'leri

```
POST /api/service-advisors/login
     Body: { email, password }
     Response: 200 OK { ServiceAdvisorDto } | 401 Unauthorized

GET  /api/service-advisors/{id}/dashboard
     Response: 200 OK [ VehicleOptionDto ] | 404 Not Found
```

## Kullanım Örnekleri

```csharp
// Uygulama katmanında oluşturma
var emailResult = Email.Create("advisor@dealership.com");
var hash = _passwordHasher.Hash("SecurePass123!");
var result = ServiceAdvisor.Create("Alvo", "Yarnsby", emailResult.Value, hash, "Satış");

// Oturum açma (LoginServiceAdvisorCommandHandler)
var advisor = await _repository.GetByEmailAsync(command.Email);
if (advisor is null || !_passwordHasher.Verify(command.Password, advisor.PasswordHash))
    return Result<ServiceAdvisorDto>.Failure("Invalid credentials");
```

## İlgili Dosyalar

| Katman | Dosya |
|---|---|
| Domain | `VehicleInventory.Domain/Entities/ServiceAdvisor.cs` |
| Application | `VehicleInventory.Application/Commands/LoginServiceAdvisorCommand.cs` |
| Application | `VehicleInventory.Application/Queries/GetAdvisorDashboardQueryHandler.cs` |
| Application | `VehicleInventory.Application/DTOs/ServiceAdvisorDto.cs` |
| Application | `VehicleInventory.Application/Abstractions/IPasswordHasher.cs` |
| Infrastructure | `VehicleInventory.Infrastructure/Repositories/ServiceAdvisorRepository.cs` |
| API | `VehicleInventory.API/Controllers/ServiceAdvisorsController.cs` |
