# US-003: Araç Opsiyonlama

## Kullanıcı Hikayesi
**Rol olarak**: Satış danışmanı  
**İstiyorum ki**: Bir müşteri adına belirli bir aracı satın alma opsiyonu ile belirli bir süre için rezerve edebileyim  
**Böylece**: Müşteri karar verme sürecinde araç başka bir müşteriye satılmasın ve ciddi alıcılar için güvence oluşsun

## Kabul Kriterleri

### AC-1: Opsiyon Oluşturma
- [ ] Satış danışmanı mevcut bir aracı ve mevcut bir müşteriyi seçerek opsiyon oluşturabilmeli
- [ ] Opsiyon süresi 1 ile 30 gün arasında belirtilmeli
- [ ] Opsiyon ücreti (depozito) sıfır veya üzeri bir tutar olarak girilmeli
- [ ] Opsiyon oluşturulduğunda araç durumu otomatik olarak `Reserved` (Rezerve) statüsüne geçmeli
- [ ] Halihazırda aktif opsiyonu olan bir araç tekrar opsiyonlanamaz
- [ ] Satılmış (`Sold`) bir araç opsiyonlanamaz

### AC-2: Opsiyon İptali
- [ ] Satış danışmanı aktif bir opsiyonu iptal edebilmeli
- [ ] İptal sonrası araç durumu `OnSale` statüsüne geri dönmeli
- [ ] Sadece `Active` statüsündeki opsiyonlar iptal edilebilmeli

### AC-3: Opsiyon Sorgulama
- [ ] Bir araca ait tüm opsiyon geçmişi listelenebilmeli
- [ ] Bir müşteriye ait tüm opsiyonlar listelenebilmeli

### AC-4: Opsiyonlama Sırasında Yeni Müşteri Oluşturma
- [ ] Müşteri arama alanı yanında "Yeni Müşteri" butonu bulunmalı
- [ ] Butona tıklandığında sayfayı terk etmeden bir Bootstrap 5 modal açılmalı
- [ ] Modal; Ad, Soyad, E-posta, Telefon alanlarını ve Müşteri Tipi (Bireysel / Kurumsal) radio seçimini içermeli
- [ ] Kurumsal seçildiğinde Firma Adı ve Vergi No alanları koşullu olarak görünmeli
- [ ] API hatası (örn. 409 Conflict — e-posta zaten kayıtlı) modal içinde inline olarak gösterilmeli
- [ ] Müşteri başarıyla oluşturulduğunda modal kapanmalı ve yeni müşteri forma otomatik olarak seçilmeli

## İş Kuralları

### BR-1: Araç Uygunluğu
Sadece `InStock` veya `OnSale` statüsündeki araçlar opsiyonlanabilir. `Reserved` (başka aktif opsiyon var) veya `Sold` araçlar opsiyonlanamaz.

### BR-2: Opsiyon Süresi
Opsiyon geçerlilik süresi minimum 1, maksimum 30 gün olarak belirlenir. Süre, opsiyonun oluşturulduğu UTC tarihinden itibaren hesaplanır.

### BR-3: Opsiyon Ücreti
Opsiyon ücreti (depozito), araçla aynı para biriminde girilmelidir. Sıfır değer geçerlidir (ücretsiz opsiyon).

### BR-4: Tekil Aktif Opsiyon
Bir araç için aynı anda yalnızca bir aktif opsiyon bulunabilir.

### BR-5: Hızlı Müşteri Kaydı
Opsiyonlama formu üzerinden yeni bir müşteri oluşturulabilir. Bu işlem aynı iş kurallarını uygular: e-posta adresi sistem genelinde benzersiz olmalıdır (409 Conflict); bireysel müşteri için ad, soyad, e-posta ve telefon zorunludur; kurumsal müşteri için ek olarak firma adı ve vergi numarası gereklidir. Başarılı kayıt sonrası yeni müşteri, sayfayı terk etmeden otomatik olarak forma seçilir.

## Teknik Notlar

### API Endpoint(s)
```
POST   /api/vehicle-options
       Body: { vehicleId, customerId, validityDays, optionFeeAmount, optionFeeCurrency, notes? }
       Response: 201 Created { id: guid }

DELETE /api/vehicle-options/{id}
       Response: 200 OK

GET    /api/vehicle-options/vehicle/{vehicleId}
       Response: 200 OK [ VehicleOptionDto ]

GET    /api/vehicle-options/customer/{customerId}
       Response: 200 OK [ VehicleOptionDto ]
```

### Domain Events
- `VehicleOptionCreated` — Opsiyon oluşturulduğunda araç `Reserved` statüsüne geçer
- `VehicleOptionCancelled` — Opsiyon iptal edildiğinde araç `OnSale` statüsüne döner

### Bağımlılıklar
- `Vehicle` aggregate root (VehicleInventory.Domain)
- `Customer` aggregate root (VehicleInventory.Domain)
- `IVehicleRepository`, `ICustomerRepository`, `IVehicleOptionRepository`

## Test Senaryoları

### Senaryo 1: Başarılı Opsiyon Oluşturma
1. `InStock` statüsünde bir araç ve mevcut bir müşteri seç
2. 7 günlük opsiyon, 5000 TRY depozito ile POST /api/vehicle-options çağır
3. 201 döner, araç statüsü `Reserved` olur

### Senaryo 2: Zaten Rezerve Araç
1. `Reserved` statüsünde bir araç seç
2. POST /api/vehicle-options çağır
3. 400 Bad Request + "Vehicle is already reserved" mesajı döner

### Senaryo 3: Opsiyon İptali
1. Aktif opsiyonu olan bir araç için DELETE /api/vehicle-options/{id} çağır
2. 200 döner, araç statüsü `OnSale` olur

### Senaryo 4: Geçersiz Süre
1. validityDays = 0 ile opsiyon oluşturmaya çalış
2. 400 Bad Request döner

## UI/UX Notları
- Araç seçimi için mevcut `InStock`/`OnSale` araçlar listelenmeli
- Müşteri arama alanının yanında **"Yeni Müşteri"** butonu (Bootstrap `btn-outline-success`, `bi-person-plus` ikon) bulunmalı
- Butona tıklandığında Bootstrap 5 modal açılarak ad, soyad, e-posta, telefon ve müşteri tipi (Bireysel / Kurumsal) alanları gösterilmeli; Kurumsal seçilince firma adı ve vergi no alanları koşullu olarak görünmeli
- Müşteri oluşturma sırasında oluşabilecek API hataları (örn. 409 Conflict) modal içinde inline alert olarak gösterilmeli
- Opsiyonun bitiş tarihi kullanıcıya gösterilmeli

## Ekran Mockup Referansı
`docs/ui/vehicle-option-form.html`

## Öncelik
**Yüksek**

## Efor Tahmini
**3 Story Point**

## Sprint
Sprint 2

## Bağımlı Story'ler
- US-001: Araç Ekleme
- US-002: Araç Listeleme
- US-004: Müşteri Yönetimi
- US-006: Araç Opsiyonlama Sırasında Hızlı Müşteri Kaydı
