# US-007: Araç Opsiyonlama Özet Listesi

## Kullanıcı Hikayesi
**Servis danışmanı olarak**,  
**müşterilere ait araç opsiyonlama kayıtlarını tek bir ekranda filtreleyerek listeleyebilmek istiyorum**,  
**Böylece müşterilerin aktif, süresi dolmuş veya iptal edilmiş opsiyonlarını hızlıca sorgulayabilir ve doğru bilgiyi müşteriye anında iletebilirim**.

## Bağlam ve Amaç

Servis danışmanları gün içinde farklı müşterilerden araç opsiyonlama durumu hakkında soru alır. US-005 kapsamındaki dashboard yalnızca danışmanın kendi oluşturduğu opsiyonları gösterirken, bu story tüm opsiyonlama kayıtlarına tek noktadan erişim imkânı sunmaktadır. Müşteri adı, araç bilgisi veya opsiyon durumuna göre anlık sorgulama yapılabilmesi danışmanın yanıt süresini kısaltır ve işlem doğruluğunu artırır.

## Kabul Kriterleri

### AC-1: Opsiyon Listesinin Görüntülenmesi
- [ ] Ekran açıldığında sistem tüm opsiyonları (Active, Expired, Cancelled) varsayılan sıralama ile listeler
- [ ] Her satırda şu bilgiler yer almalı: müşteri görünen adı, araç adı (Marka Model Yıl), araç VIN'i, opsiyon durumu (badge), bitiş tarihi, opsiyon ücreti, opsiyonu oluşturan servis danışmanı adı
- [ ] Liste varsayılan olarak bitiş tarihine göre artan sırada (en yakın üstte) gösterilmeli
- [ ] Liste sayfalama (pagination) desteklemeli; sayfa başına varsayılan 20 kayıt

### AC-2: Filtreleme
- [ ] Müşteri adı veya soyadı ile serbest metin araması yapılabilmeli (case-insensitive, kısmi eşleşme)
- [ ] Araç markası, modeli veya VIN değerine göre serbest metin araması yapılabilmeli
- [ ] Opsiyon durumuna göre filtreleme yapılabilmeli: Tümü / Aktif / Süresi Dolmuş / İptal Edilmiş
- [ ] Opsiyonun oluşturulma tarihine göre başlangıç–bitiş aralığı filtrelemesi uygulanabilmeli
- [ ] Birden fazla filtre aynı anda uygulanabilmeli
- [ ] Filtreler temizlendiğinde liste tam listeye geri dönmeli

### AC-3: Durum Gösterimi
- [ ] `Active` opsiyonlar yeşil badge ile gösterilmeli
- [ ] `Expired` opsiyonlar sarı/turuncu badge ile gösterilmeli
- [ ] `Cancelled` opsiyonlar gri badge ile gösterilmeli
- [ ] `ExpiresAt` değeri mevcut UTC zamanından küçük ve durum `Active` ise kayıt `Expired` olarak gösterilmeli (veritabanı güncellenmeden, sorgulama anında hesaplanır)

### AC-4: Boş Durum ve Hata Yönetimi
- [ ] Filtre sonucunda kayıt bulunamazsa "Opsiyonlama kaydı bulunamadı" mesajı gösterilmeli
- [ ] API hatası durumunda kullanıcıya bilgi veren bir hata mesajı gösterilmeli; liste alanı boş bırakılmamalı

### AC-5: Arama Performansı
- [ ] Liste ilk yükleme süresi 2 saniyeyi geçmemeli
- [ ] Filtre uygulandıktan sonra sonuç 1 saniye içinde güncellenmeli

## İş Kuralları

### BR-1: Görüntüleme Yetkisi
Servis danışmanı, tüm danışmanlara ait opsiyonları görebilir; yalnızca kendi oluşturduklarıyla kısıtlı değildir. Bu kural US-005 dashboard'undan bu story'yi ayırt eden temel iş kuralıdır.

### BR-2: Süresi Dolmuş Opsiyon Hesabı
`ExpiresAt < UTC Now` koşulunu sağlayan ve veritabanında hâlâ `Active (1)` olarak saklanan kayıtlar, API'nin sorgu yanıtında `Status = Expired` olarak dönüştürülür. Veritabanı kaydı değiştirilmez; bu hesaplama sorgulama anında yapılır.

### BR-3: Sıralama
Varsayılan sıralama bitiş tarihine göre artan sıradadır (yaklaşan son tarihler üstte). Kullanıcı sıralamayı tablodaki sütun başlıklarına tıklayarak değiştirebilir.

### BR-4: Sayfalama
Sayfa başına döndürülen kayıt sayısı varsayılan 20'dir; istemci `pageSize` parametresiyle 10, 20 veya 50 olarak ayarlayabilir. Toplam kayıt sayısı ve toplam sayfa adedi yanıtta yer alır.

### BR-5: Serbest Metin Arama
Müşteri adı araması `FirstName` ve `LastName` alanları üzerinde yapılır. Araç araması `Brand`, `Model` ve `VIN` alanları üzerinde yapılır. Arama büyük/küçük harf duyarsız ve kısmi eşleşmeye (ILIKE) izin verir.

## Kapsam Dışı (Out of Scope)
- Bu ekrandan opsiyon oluşturma veya iptal işlemi yapılamaz (yalnızca görüntüleme)
- Müşteri veya araç detay sayfasına navigasyon (bağlantı) bu story kapsamında değildir
- Excel/PDF dışa aktarma bu story kapsamında değildir
- Rol tabanlı erişim kontrolü (sadece belirli danışmanların görmesi) bu story kapsamında değildir

## Bağımlılıklar
- **US-003**: VehicleOption oluşturma — listelenen verinin kaynağı
- **US-004**: Customer yönetimi — müşteri görünen adı için
- **US-005**: ServiceAdvisor dashboard — aynı `VehicleOptionDto` yapısını paylaşır
- `GET /api/vehicle-options` endpoint'i (filtre parametreli) mevcut değilse geliştirilmeli

## Test Senaryoları

### Senaryo 1: Tüm Opsiyonları Listeleme (Happy Path)
**Koşul**: Sistemde en az 3 farklı durumda (Active, Expired, Cancelled) opsiyon kaydı mevcut  
**Adımlar**:
1. Servis danışmanı özet liste ekranını açar
2. Herhangi bir filtre uygulamaz
3. "Listele" butonuna tıklar veya sayfa otomatik yüklenir  
**Beklenen sonuç**: Tüm opsiyonlar bitiş tarihine göre artan sırada, durum badge'leriyle listelenir; sayfalama kontrolü görünür

### Senaryo 2: Müşteri Adına Göre Filtreleme
**Koşul**: "John Doe" adlı müşteriye ait aktif bir opsiyon kayıtlı  
**Adımlar**:
1. Danışman müşteri arama kutusuna "John" yazar
2. Filtre uygulanır  
**Beklenen sonuç**: Sadece "John Doe"a ait opsiyonlar listelenir; diğer müşterilerin opsiyonları görünmez

### Senaryo 3: Durum Filtrelemesi — Yalnızca Aktif
**Koşul**: Active, Expired ve Cancelled opsiyonlar mevcut  
**Adımlar**:
1. Durum filtresi "Aktif" olarak seçilir  
**Beklenen sonuç**: Yalnızca `Status = Active` (ve süresi dolmamış) opsiyonlar gösterilir

### Senaryo 4: Süresi Dolmuş Opsiyon Gösterimi
**Koşul**: `ExpiresAt` değeri geçmiş olan ve veritabanında `Active` statüsünde kaydedilmiş bir opsiyon mevcut  
**Adımlar**:
1. Danışman listeyi açar veya durum filtresi "Süresi Dolmuş"u seçer  
**Beklenen sonuç**: Söz konusu opsiyon listede `Expired` badge'iyle görünür; veritabanında güncelleme yapılmaz

### Senaryo 5: Sonuç Bulunamadı
**Koşul**: Sistemde "XYZ" adlı müşteri yok  
**Adımlar**:
1. Müşteri arama kutusuna "XYZ" yazılır  
**Beklenen sonuç**: "Opsiyonlama kaydı bulunamadı" mesajı gösterilir; sayfa hata vermez

### Senaryo 6: Sayfalama
**Koşul**: Sistemde 45 opsiyon kaydı mevcut  
**Adımlar**:
1. Sayfa 1 yüklenir (varsayılan pageSize = 20)
2. Sayfa 2'ye geçilir  
**Beklenen sonuç**: Sayfa 1'de 20, sayfa 2'de 20, sayfa 3'te 5 kayıt gösterilir; toplam "45 kayıt" bilgisi görünür

## UI Referansı
- `docs/ui/vehicle-option-summary-list.html` — Wireframe referansı (oluşturulacak)

## Domain Referansları
- Entity: `docs/domain-model/entity-vehicle-option.md`
- Entity: `docs/domain-model/entity-customer.md`
- Entity: `docs/domain-model/entity-vehicle.md`
- Entity: `docs/domain-model/entity-service-advisor.md`
- Value Object: `docs/domain-model/value-object-money.md`
- Value Object: `docs/domain-model/value-object-vin.md`

## Teknik Notlar

### API Endpoint

```
GET /api/vehicle-options
    Query Parameters:
      customerSearch  string?   — Müşteri adı/soyadı serbest metin
      vehicleSearch   string?   — Araç markası, modeli veya VIN
      status          int?      — 1=Active, 2=Expired, 3=Cancelled (boşsa tümü)
      createdFrom     datetime? — Oluşturulma tarihi başlangıcı (UTC)
      createdTo       datetime? — Oluşturulma tarihi bitişi (UTC)
      page            int       — Varsayılan: 1
      pageSize        int       — 10 | 20 | 50, Varsayılan: 20
      sortBy          string?   — "expiresAt" | "createdAt" | "customerName", Varsayılan: "expiresAt"
      sortDirection   string?   — "asc" | "desc", Varsayılan: "asc"
    Response: 200 OK PagedResult<VehicleOptionSummaryDto>
```

### DTO: VehicleOptionSummaryDto

```json
{
  "id": "guid",
  "vehicleId": "guid",
  "vehicleDisplayName": "Honda Civic 2026",
  "vehicleVIN": "1HGBH41JXMN109186",
  "customerId": "guid",
  "customerDisplayName": "John Doe",
  "serviceAdvisorId": "guid",
  "serviceAdvisorDisplayName": "Serko Havrick",
  "expiresAt": "2026-06-01T00:00:00Z",
  "optionFeeAmount": 5000.00,
  "optionFeeCurrency": "TRY",
  "notes": "...",
  "status": 1,
  "isExpired": false,
  "createdAt": "2026-05-08T10:00:00Z"
}
```

### DTO: PagedResult\<T\>

```json
{
  "items": [ VehicleOptionSummaryDto ],
  "totalCount": 45,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

### Query: GetVehicleOptionSummaryQuery

```csharp
public record GetVehicleOptionSummaryQuery(
    string? CustomerSearch,
    string? VehicleSearch,
    VehicleOptionStatus? Status,
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "expiresAt",
    string SortDirection = "asc"
) : IRequest<PagedResult<VehicleOptionSummaryDto>>;
```

### Bağımlılıklar
- `IVehicleOptionRepository` — `GetSummaryAsync(query)` metodu eklenmeli
- `VehicleOptionSummaryDto` — yeni DTO (mevcut `VehicleOptionDto`'dan türetilir veya ayrı tanımlanır)
- `PagedResult<T>` — genel sayfalama sarmalayıcısı (mevcut değilse Application katmanına eklenmeli)
- `Vehicle`, `Customer`, `ServiceAdvisor` JOIN'leri Infrastructure sorgusunda yapılır

## Tahmini Büyüklük
[ ] XS (< 2 saat) [ ] S (yarım gün) [x] M (1 gün) [ ] L (2-3 gün) [ ] XL (1 hafta+)

## Öncelik
**Orta**

## Efor Tahmini
**3 Story Point**

## Sprint
Sprint 3

## Bağımlı Story'ler
- US-003: Araç Opsiyonlama (veri kaynağı)
- US-004: Müşteri Yönetimi (müşteri bilgisi)
- US-005: Servis Danışmanı Girişi ve Dashboard (paylaşılan DTO yapısı)
