# US-004: Müşteri Yönetimi

## Kullanıcı Hikayesi
**Rol olarak**: Satış danışmanı  
**İstiyorum ki**: Müşteri kayıtlarını oluşturabileyim, listeleyebileyim ve detaylarını görüntüleyebileyim  
**Böylece**: Araç opsiyonlama ve satış süreçlerinde doğru müşteri bilgilerini kullanabileyim

## Kabul Kriterleri

### AC-1: Müşteri Oluşturma
- [ ] Bireysel müşteri için ad, soyad, e-posta, telefon alanları zorunlu olmalı
- [ ] Kurumsal müşteri için ek olarak şirket adı ve vergi numarası girilmeli
- [ ] Aynı e-posta adresiyle ikinci bir müşteri oluşturulamaz
- [ ] E-posta adresi RFC 5322 formatında doğrulanmalı (max 254 karakter)
- [ ] Başarılı oluşturmada 201 Created ve yeni kaydın `id`'si döner

### AC-2: Müşteri Listeleme
- [ ] Tüm müşteriler sayfalı olarak listelenebilmeli (varsayılan: sayfa 1, sayfa boyutu 10)
- [ ] Ad, soyad, e-posta veya şirket adına göre serbest metin arama yapılabilmeli
- [ ] `CustomerType` (Individual / Corporate) filtresi uygulanabilmeli
- [ ] Her kayıtta `DisplayName` (bireysel: "Ad Soyad", kurumsal: "Şirket Adı") gösterilmeli

### AC-3: Müşteri Detayı
- [ ] GUID ile tek müşteri bilgisi getirilebilmeli
- [ ] Bulunamayan GUID için 404 döner

## İş Kuralları

### BR-1: E-posta Benzersizliği
E-posta adresi sistem genelinde unique olmalıdır. Var olan bir adresle kayıt oluşturulmaya çalışıldığında 409 Conflict döner.

### BR-2: Müşteri Tipi
`CustomerType.Individual` (1) veya `CustomerType.Corporate` (2) değerlerinden biri seçilmeli. Kurumsal kayıtlar için `CompanyName` boş olamaz.

### BR-3: Görüntüleme Adı
- Bireysel: `"FirstName LastName"`
- Kurumsal: `"CompanyName"`

## Teknik Notlar

### API Endpoint(s)
```
POST /api/v1/customers
     Body: { firstName, lastName, email, phone, customerType, companyName?, taxNumber? }
     Response: 201 Created { id: guid } | 400 Bad Request | 409 Conflict

GET  /api/v1/customers
     Query: ?page=1&pageSize=10&search=&customerType=
     Response: 200 OK PagedResult<CustomerDto>

GET  /api/v1/customers/{id:guid}
     Response: 200 OK CustomerDto | 404 Not Found
```

### Bağımlılıklar
- `Email` value object (`VehicleInventory.Domain/ValueObjects/Email.cs`)
- `ICustomerRepository` (`GetByEmailAsync`, `ExistsByEmailAsync`, `GetAllAsync`, `AddAsync`)
- `CustomerType` enum

## Test Senaryoları

### Senaryo 1: Bireysel Müşteri Oluşturma
1. `firstName`, `lastName`, `email`, `phone`, `customerType: 1` ile POST isteği gönder
2. 201 ve yeni GUID döner

### Senaryo 2: Aynı E-posta ile Kayıt
1. Var olan bir e-posta ile POST isteği gönder
2. 409 Conflict döner

### Senaryo 3: Müşteri Listesi ve Arama
1. GET /api/v1/customers?search=Ahmet çağır
2. Ad, soyad veya şirket adında "Ahmet" geçen kayıtlar döner

### Senaryo 4: Geçersiz E-posta
1. `email: "geçersiz"` ile POST isteği gönder
2. 400 Bad Request döner

## UI/UX Notları
- Listeleme ekranında tip rozeti (Bireysel / Kurumsal) gösterilmeli
- Kurumsal kayıt formunda `companyName` ve `taxNumber` alanları görünmeli

## Öncelik & Planlama

| Alan | Değer |
|---|---|
| Öncelik | Yüksek |
| Story Points | 5 |
| Sprint | Sprint 2 |
| Bağımlı Hikayeler | US-003 (VehicleOption müşteri seçimi) |
