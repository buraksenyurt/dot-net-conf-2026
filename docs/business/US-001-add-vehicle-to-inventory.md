# US-001: Araç Envanterine Yeni Araç Ekleme

## Kullanıcı Hikayesi
**Rol olarak**: Bayi stok yöneticisi  
**İstiyorum ki**: Sistemde yeni alınan araçları envantere ekleyebileyim  
**Böylece**: Stok takibi yapabileyim ve satış ekibine sunabilirim

## Kabul Kriterleri

### AC-1: Temel Araç Bilgileri Girişi
- [ ] VIN (Vehicle Identification Number) zorunlu alan olmalı
- [ ] VIN 17 karakter uzunluğunda olmalı
- [ ] Marka, model ve yıl bilgisi zorunlu olmalı
- [ ] Motor tipi seçimi yapılabilmeli (Benzin, Dizel, Elektrik, Hibrit)
- [ ] Kilometre bilgisi girilmeli (varsayılan: 0)
- [ ] Renk bilgisi girilmeli

### AC-2: Fiyat Bilgileri
- [ ] Alış fiyatı girilmeli
- [ ] Tavsiye edilen satış fiyatı girilmeli
- [ ] Para birimi seçimi yapılabilmeli (TRY, USD, EUR)
- [ ] Fiyatlar pozitif değer olmalı

### AC-3: Donatım ve Özellikler
- [ ] Vites tipi seçilmeli (Manuel, Otomatik, Yarı Otomatik)
- [ ] Yakıt tüketim bilgisi girilmeli
- [ ] Motor hacmi girilmeli (cc)
- [ ] Opsiyonel donatımlar eklenebilmeli (Sunroof, Deri Döşeme, vs.)

### AC-4: Validasyonlar
- [ ] Aynı VIN numarasına sahip araç zaten varsa hata mesajı gösterilmeli
- [ ] Tüm zorunlu alanlar doldurulmadan kayıt yapılamamalı
- [ ] Geçersiz VIN formatı için uyarı verilmeli
- [ ] Araç yılı mevcut yıldan büyük olamaz

### AC-5: Başarılı Kayıt Sonrası
- [ ] Başarılı kayıt mesajı gösterilmeli
- [ ] Yeni eklenen araç envanter listesinde görünmeli
- [ ] Araç durumu "Stokta" olarak işaretlenmeli
- [ ] Sistem logu kaydedilmeli

## İş Kuralları

### BR-1: VIN Doğrulama
VIN numarası ISO 3779 standardına uygun olmalıdır:
- Toplam 17 karakter
- I, O, Q harfleri kullanılmaz
- Check digit algoritması ile doğrulanmalı

### BR-2: Fiyat Hesaplama
- Tavsiye satış fiyatı, alış fiyatından düşük olamaz
- Varsayılan kar marjı %15 olarak hesaplanır
- Farklı para birimleri güncel kurdan dönüştürülür

### BR-3: Stok Durumu
Yeni eklenen araç otomatik olarak:
- Durum: "Stokta"
- Konum: Bayi ana deposu
- Gösterim için hazır: Hayır

## Teknik Notlar

### API Endpoint
```
POST /api/v1/vehicles
Content-Type: application/json
Authorization: Bearer {token}

Request Body:
{
  "vin": "1HGBH41JXMN109186",
  "brand": "Honda",
  "model": "Civic",
  "year": 2026,
  "engineType": "Hybrid",
  "mileage": 0,
  "color": "Gümüş",
  "purchasePrice": {
    "amount": 1500000,
    "currency": "TRY"
  },
  "suggestedPrice": {
    "amount": 1725000,
    "currency": "TRY"
  },
  "transmissionType": "Automatic",
  "fuelConsumption": "4.8",
  "engineCapacity": 1500,
  "features": ["Sunroof", "LeatherSeats", "NavigationSystem"]
}
```

### Domain Events
- `VehicleAddedToInventoryEvent` - Araç envantere eklendiğinde
- `VehicleValidatedEvent` - VIN doğrulaması yapıldığında

### Bağımlılıklar
- Marka/Model listesi static data'dan gelir
- Para birimi dönüşümü için döviz kuru servisi kullanılır
- VIN doğrulama için harici validasyon servisi kullanılabilir

## Test Senaryoları

### Senaryo 1: Başarılı Araç Ekleme
1. Kullanıcı yeni araç formunu açar
2. Tüm zorunlu alanları doldurur
3. "Kaydet" butonuna tıklar
4. Sistem VIN'i doğrular
5. Araç envantere eklenir
6. Başarı mesajı gösterilir

### Senaryo 2: Duplicate VIN Hatası
1. Kullanıcı daha önce eklenmiş bir VIN ile araç eklemeye çalışır
2. Sistem VIN kontrolü yapar
3. "Bu VIN numarası zaten sistemde kayıtlı" hatası gösterilir
4. Kayıt işlemi gerçekleşmez

### Senaryo 3: Geçersiz VIN Formatı
1. Kullanıcı 15 karakterlik VIN girer
2. Format validasyonu çalışır
3. "VIN numarası 17 karakter olmalıdır" uyarısı gösterilir
4. Form submit edilemez

## Ekran Mockup Referansı
`docs/ui/vehicle-add-form.html`

## Öncelik
**Yüksek** - MVP'nin kritik özelliği

## Efor Tahmini
**5 Story Point**

## Sprint
Sprint 1

## Bağımlı Story'ler
- US-002: Araç Listeleme (Listeleme için önce ekleme olmalı)
