# Domain Model: Customer (Müşteri)

## Entity Tanımı

**Customer** - Araç satın alabilen gerçek veya tüzel kişiyi temsil eden aggregate root entity.

## Sorumluluklar
- Müşteri kimlik ve iletişim bilgilerini saklamak ve yönetmek
- Bireysel ve kurumsal müşteri ayrımını enforce etmek
- E-posta benzersizliğini koruyan iş kurallarını uygulamak

## CustomerType (Enum)

| Değer | Açıklama |
|---|---|
| `Individual` | Bireysel müşteri (gerçek kişi) |
| `Corporate` | Kurumsal müşteri (şirket / tüzel kişilik) |

## Properties

### Identity
- **Id**: `Guid` — Unique identifier (Primary Key)

### Kişi Bilgileri
- **FirstName**: `string` — İsim (max 100 karakter)
- **LastName**: `string` — Soyisim (max 100 karakter)

### İletişim
- **Email**: `Email (Value Object)` — E-posta adresi (benzersiz)
- **Phone**: `string` — Telefon numarası (max 30 karakter)

### Müşteri Tipi
- **CustomerType**: `CustomerType (Enum)` — Bireysel / Kurumsal

### Kurumsal Bilgiler *(sadece Corporate)*
- **CompanyName**: `string?` — Şirket adı (max 200 karakter)
- **TaxNumber**: `string?` — Vergi numarası (max 50 karakter)

### Audit
- **CreatedAt**: `DateTime` — Oluşturulma tarihi (UTC)
- **UpdatedAt**: `DateTime?` — Son güncellenme tarihi (UTC, nullable)

## Factory Methods (Domain Methods)

### `CreateIndividual`
Bireysel müşteri oluşturur. `CompanyName` ve `TaxNumber` otomatik olarak `null` bırakılır.

```csharp
Customer.CreateIndividual(
    firstName: "Alvo",
    lastName: "Yarnsby",
    email: emailValueObject,
    phone: "+90 532 000 0000"
) // → Result<Customer>
```

### `CreateCorporate`
Kurumsal müşteri oluşturur. `CompanyName` ve `TaxNumber` zorunludur.

```csharp
Customer.CreateCorporate(
    firstName: "Borko",
    lastName: "Vexler",
    email: emailValueObject,
    phone: "+90 212 000 0000",
    companyName: "Vexler Otomotiv A.Ş.",
    taxNumber: "1234567890"
) // → Result<Customer>
```

### `UpdateContact`
Müşterinin iletişim bilgilerini günceller.

### `UpdateCorporateInfo`
Kurumsal müşterilerde şirket adı ve vergi numarasını günceller.  
Bireysel müşterilerde çağrılırsa hata döner.

### `GetDisplayName`
Görüntüleme için birleştirilmiş isim döner:
- **Individual**: `"John Doe"`
- **Corporate**: `"Vexler Otomotiv A.Ş. (Borko Vexler)"`

## İş Kuralları

| Kural | Açıklama |
|---|---|
| E-posta benzersiz olmalı | Aynı e-posta ile iki müşteri oluşturulamaz |
| Zorunlu alanlar boş olamaz | `FirstName`, `LastName`, `Phone` mutlaka dolu olmalı |
| Kurumsal alanlar | `CompanyName` ve `TaxNumber` yalnızca `Corporate` müşterilerde zorunlu |
| `UpdateCorporateInfo` kısıtı | Sadece `Corporate` müşterilerde çağrılabilir |

## Veritabanı (PostgreSQL)

```sql
CREATE TABLE "Customers" (
    "Id"           UUID                      NOT NULL PRIMARY KEY,
    "FirstName"    VARCHAR(100)              NOT NULL,
    "LastName"     VARCHAR(100)              NOT NULL,
    "Email"        VARCHAR(254)              NOT NULL UNIQUE,
    "Phone"        VARCHAR(30)               NOT NULL,
    "CustomerType" VARCHAR(20)               NOT NULL,
    "CompanyName"  VARCHAR(200),
    "TaxNumber"    VARCHAR(50),
    "CreatedAt"    TIMESTAMP WITH TIME ZONE  NOT NULL,
    "UpdatedAt"    TIMESTAMP WITH TIME ZONE
);

CREATE INDEX IX_Customers_CustomerType       ON "Customers" ("CustomerType");
CREATE INDEX IX_Customers_LastName_FirstName ON "Customers" ("LastName", "FirstName");
```

## İlgili Dosyalar

| Katman | Dosya |
|---|---|
| Domain — Entity | `VehicleInventory.Domain/Entities/Customer.cs` |
| Domain — Enum | `VehicleInventory.Domain/Enums/CustomerType.cs` |
| Domain — Value Object | `VehicleInventory.Domain/ValueObjects/Email.cs` |
| Domain — Interface | `VehicleInventory.Domain/Interfaces/ICustomerRepository.cs` |
| Application — Command | `VehicleInventory.Application/Commands/CreateCustomerCommand.cs` |
| Application — Queries | `VehicleInventory.Application/Queries/GetCustomersQuery.cs`, `GetCustomerByIdQuery.cs` |
| Application — DTO | `VehicleInventory.Application/DTOs/CustomerDto.cs` |
| Infrastructure — Config | `VehicleInventory.Infrastructure/Persistence/Configurations/CustomerConfiguration.cs` |
| Infrastructure — Repo | `VehicleInventory.Infrastructure/Repositories/CustomerRepository.cs` |
| API — Controller | `VehicleInventory.API/Controllers/CustomersController.cs` |
| Migration | `VehicleInventory.Infrastructure/Migrations/20260221120858_AddCustomerEntity.cs` |
