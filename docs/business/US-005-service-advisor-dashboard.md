# US-005: Servis Danışmanı Girişi ve Dashboard

## Kullanıcı Hikayesi
**Rol olarak**: Servis danışmanı  
**İstiyorum ki**: Sisteme e-posta ve parola ile giriş yapabileyim ve kendime atanmış araç opsiyonlarını tek ekranda görebileyim  
**Böylece**: Sorumlu olduğum müşteri rezervasyonlarını takip edebileyim ve müşterilere doğru bilgi verebileyim

## Kabul Kriterleri

### AC-1: Servis Danışmanı Girişi
- [ ] Danışman e-posta ve parola ile giriş yapabilmeli
- [ ] Hatalı kimlik bilgilerinde 401 Unauthorized döner; güvenlik için "e-posta bulunamadı" ya da "parola yanlış" şeklinde ayrıştırma yapılmaz
- [ ] Başarılı girişte danışmanın id, ad, soyad, departman bilgisi döner
- [ ] Pasif (`IsActive: false`) danışman giriş yapamamalı

### AC-2: Danışman Dashboard'u
- [ ] Giriş yapmış danışmana ait tüm `VehicleOption` kayıtları listelenebilmeli
- [ ] Her opsiyon kaydında araç adı/VIN, müşteri adı, bitiş tarihi, opsiyon ücreti ve durum bilgisi yer almalı
- [ ] Var olmayan danışman ID için 404 döner

## İş Kuralları

### BR-1: Kimlik Doğrulama
Parola karşılaştırması `IPasswordHasher.Verify(plainPassword, storedHash)` ile yapılır. Hash işlemi için Bcrypt kullanılır. Hatalı kimlik bilgisi durumunda kullanıcı numaralandırmasını önlemek amacıyla jenerik hata mesajı döndürülür.

### BR-2: Pasif Danışman Kısıtlaması
`IsActive: false` olan danışman login endpoint'inden 401 yanıtı alır.

### BR-3: Dashboard Kapsamı
Dashboard, yalnızca `ServiceAdvisorId` alanı o danışmana ait olan opsiyonları döndürür. `ServiceAdvisorId` null olan opsiyonlar dahil edilmez.

## Teknik Notlar

### API Endpoint(s)
```
POST /api/service-advisors/login
     Body: { email, password }
     Response: 200 OK ServiceAdvisorDto | 401 Unauthorized

GET  /api/service-advisors/{id:guid}/dashboard
     Response: 200 OK [ VehicleOptionDto ] | 404 Not Found
```

### DTO: ServiceAdvisorDto
```json
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "department": "string",
  "isActive": true
}
```

### DTO: VehicleOptionDto (dashboard listesinde)
```json
{
  "id": "guid",
  "vehicleId": "guid",
  "vehicleDisplayName": "Honda Civic 2026",
  "vehicleVIN": "1HGBH41JXMN109186",
  "customerId": "guid",
  "customerDisplayName": "John Doe",
  "expiresAt": "2026-03-15T00:00:00Z",
  "optionFeeAmount": 5000,
  "optionFeeCurrency": "TRY",
  "notes": "...",
  "status": 1,
  "isExpired": false,
  "createdAt": "...",
  "serviceAdvisorId": "guid",
  "serviceAdvisorDisplayName": "Serko Havrick"
}
```

### Bağımlılıklar
- `IServiceAdvisorRepository` (`GetByEmailAsync`, `GetByIdAsync`)
- `IVehicleOptionRepository` (`GetByServiceAdvisorIdAsync`)
- `IPasswordHasher` (`Verify`)

## Test Senaryoları

### Senaryo 1: Başarılı Giriş
1. Geçerli e-posta ve parola ile POST /api/service-advisors/login çağır
2. 200 OK ve `ServiceAdvisorDto` döner

### Senaryo 2: Hatalı Parola
1. Yanlış parola ile giriş dene
2. 401 Unauthorized döner

### Senaryo 3: Pasif Danışman Girişi
1. `IsActive: false` olan danışman bilgileriyle giriş dene
2. 401 Unauthorized döner

### Senaryo 4: Dashboard Listeleme
1. GET /api/service-advisors/{id}/dashboard çağır
2. Danışmana ait opsiyon listesi döner

### Senaryo 5: Geçersiz Danışman ID
1. Var olmayan GUID ile dashboard çağır
2. 404 Not Found döner

## UI/UX Notları
- Dashboard sayfasında opsiyonlar bitiş tarihine göre sıralanmalı (yakın olanlar üstte)
- `IsExpired: true` olan opsiyonlar farklı renkle vurgulanmalı

## Öncelik & Planlama

| Alan | Değer |
|---|---|
| Öncelik | Yüksek |
| Story Points | 3 |
| Sprint | Sprint 2 |
| Bağımlı Hikayeler | US-003 (VehicleOption oluşturma) |
