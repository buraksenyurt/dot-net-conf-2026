# US-002: Araç Envanter Listesini Görüntüleme

## Kullanıcı Hikayesi
**Rol olarak**: Bayi satış danışmanı  
**İstiyorum ki**: Envanterdeki tüm araçları listeleyip filtreleyebileyim  
**Böylece**: Müşteri ihtiyaçlarına uygun araç bulabileyim

## Kabul Kriterleri

### AC-1: Liste Görünümü
- [ ] Tüm stokta olan araçlar listelenebilmeli
- [ ] Her araç için: Marka, Model, Yıl, Renk, Fiyat, Durum bilgileri görünmeli
- [ ] Liste sayfalama yapısında olmalı (varsayılan: 20 kayıt/sayfa)
- [ ] Liste yüklenirken loading göstergesi olmalı

### AC-2: Filtreleme
- [ ] Marka bazında filtreleme yapılabilmeli
- [ ] Model bazında filtreleme yapılabilmeli
- [ ] Yıl aralığı filtrelemesi yapılabilmeli (örn: 2022-2026)
- [ ] Fiyat aralığı filtrelemesi yapılabilmeli
- [ ] Motor tipi filtrelemesi yapılabilmeli
- [ ] Durum bazında filtreleme yapılabilmeli (Stokta, Rezerve, Satıldı)

### AC-3: Sıralama
- [ ] Fiyata göre artan/azalan sıralama
- [ ] Tarihe göre sıralama (Yeni eklenenden eskiye)
- [ ] Kilometre bazında sıralama
- [ ] Marka/Model alfabetik sıralama

### AC-4: Arama
- [ ] VIN numarası ile arama yapılabilmeli
- [ ] Marka/Model ismi ile arama yapılabilmeli
- [ ] Arama sonuçları anlık görünmeli (debounce: 500ms)

### AC-5: Detay Görünümü
- [ ] Liste üzerinden araç detayına gidilebilmeli
- [ ] Detay sayfasında tüm araç bilgileri görünmeli
- [ ] Araç fotoğrafları görüntülenebilmeli

## İş Kuralları

### BR-1: Görüntüleme Yetkisi
- Satış danışmanı sadece kendi bayisinin araçlarını görebilir
- Bölge müdürü bölgesindeki tüm bayilerin araçlarını görebilir
- Sistem yöneticisi tüm araçları görebilir

### BR-2: Durum Gösterimi
- **Stokta**: Yeşil badge
- **Rezerve**: Sarı badge
- **Satıldı**: Gri badge
- **Serviste**: Mavi badge

### BR-3: Performans
- Liste ilk yükleme max 2 saniye içinde tamamlanmalı
- Filtreleme işlemleri max 1 saniye içinde sonuç dönmeli
- Sayfalama lazy loading ile yapılmalı

## Teknik Notlar

### API Endpoint
```
GET /api/v1/vehicles?page=1&pageSize=20&brand=Honda&sortBy=price&sortOrder=asc
Authorization: Bearer {token}

Response:
{
  "data": [
    {
      "id": "uuid",
      "vin": "1HGBH41JXMN109186",
      "brand": "Honda",
      "model": "Civic",
      "year": 2026,
      "color": "Gümüş",
      "price": {
        "amount": 1725000,
        "currency": "TRY"
      },
      "status": "Available",
      "mileage": 0,
      "addedDate": "2026-02-01T10:30:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalPages": 5,
    "totalCount": 95,
    "pageSize": 20
  }
}
```

### Query Parameters
- `page`: Sayfa numarası
- `pageSize`: Sayfa başına kayıt sayısı
- `brand`: Marka filtresi
- `model`: Model filtresi
- `minYear`, `maxYear`: Yıl aralığı
- `minPrice`, `maxPrice`: Fiyat aralığı
- `status`: Durum filtresi
- `engineType`: Motor tipi
- `sortBy`: Sıralama alanı
- `sortOrder`: asc/desc
- `search`: Genel arama terimi

### Frontend State Management
```typescript
// Pinia Store
interface VehicleListState {
  vehicles: Vehicle[]
  loading: boolean
  filters: VehicleFilters
  pagination: Pagination
  selectedVehicle: Vehicle | null
}
```

## Test Senaryoları

### Senaryo 1: Liste İlk Yükleme
1. Kullanıcı araç listesi sayfasını açar
2. Sistem varsayılan olarak ilk 20 aracı getirir
3. Liste görüntülenir
4. Sayfalama bilgisi gösterilir

### Senaryo 2: Filtreleme
1. Kullanıcı marka filtresinden "Honda" seçer
2. Sistem sadece Honda araçları getirir
3. Liste güncellenir
4. Filtre badge'i gösterilir

### Senaryo 3: VIN ile Arama
1. Kullanıcı arama kutusuna VIN numarası girer
2. 500ms sonra arama tetiklenir
3. Eşleşen araç bulunursa gösterilir
4. Bulunamazsa "Araç bulunamadı" mesajı gösterilir

## UI/UX Notları
- Responsive tasarım (mobile, tablet, desktop)
- Filtreler mobilde drawer içinde açılmalı
- Skeleton loader kullanılmalı
- Infinite scroll opsiyonel olarak eklenebilir

## Ekran Mockup Referansı
`docs/ui/vehicle-list-page.html`

## Öncelik
**Yüksek** - MVP kritik özellik

## Efor Tahmini
**8 Story Point**

## Sprint
Sprint 1

## Bağımlılıklar
- US-001: Araç Ekleme (Liste için veri olmalı)
