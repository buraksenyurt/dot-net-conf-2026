---
name: validate-wireframe
description: docs/ui/ altındaki HTML wireframe dosyalarını kalite kontrol açısından denetleme. Semantic HTML doğruluğu, WCAG 2.1 AA erişilebilirlik, heading hiyerarşisi, mevcut pattern tutarlılığı ve user story kapsama kontrollerini yapar; sorunları önceliklendirilmiş raporla çıktılar.
---

# Skill: Wireframe Validasyon ve Kalite Kontrol

## Ne Zaman Kullanılır
- Yeni oluşturulan bir wireframe teslim öncesi denetlenecekse
- Mevcut bir wireframe revize edildikten sonra regresyon kontrolü yapılacaksa
- `create-wireframe` skill'i ile üretilen dosya doğrulanacaksa
- User story'nin tüm AC maddelerinin wireframe'e yansıyıp yansımadığı kontrol edilecekse

## Gerekli Context Dosyaları
- `docs/ui/[hedef-wireframe].html` — Denetlenecek dosya
- `docs/business/US-XXX-*.md` — Kapsam kontrolü için user story
- `docs/ui/*.html` — Pattern tutarlılığı için diğer wireframe'ler

## Denetim Kategorileri

### 1. Semantic HTML — Kritik (P0)

Aşağıdaki kurallardan herhangi biri ihlallenirse wireframe kabul **edilmez**:

| Kural | Kontrol |
|---|---|
| Heading hiyerarşisi | `<h1>` → `<h2>` → `<h3>` sırasıyla, atlama yok |
| Semantic element | `<header>`, `<nav>`, `<main>`, `<section>`, `<article>`, `<footer>` doğru kullanılmış |
| Tablo sadece veri için | `<table>` layout amacıyla kullanılmamış |
| `<button>` vs `<a>` | Aksiyonlar `<button>`, navigasyon `<a>` |
| Form ilişkilendirme | Her `<input>` / `<select>` / `<textarea>` için eşleşen `<label for="...">` |
| Dil tanımı | `<html lang="tr">` mevcut |
| Charset | `<meta charset="UTF-8">` mevcut |

**Kontrol yöntemi:**
```
# Heading atlamasını bul
grep -n "<h[0-9]" dosya.html

# Label-input eşleşmesini kontrol et
grep -n "<input\|<label" dosya.html

# Layout için table kullanımını tespit et
grep -n "<table" dosya.html  # Ardından içeriği incele
```

### 2. Erişilebilirlik — WCAG 2.1 AA (P0)

| WCAG Kriteri | Kontrol |
|---|---|
| 1.1.1 Non-text Content | Tüm `<img>` için `alt` attribute zorunlu |
| 1.3.1 Info and Relationships | `<table>` için `scope="col/row"`, `aria-label` veya `<caption>` |
| 2.1.1 Keyboard | Interactive elementler (button, a, input) klavye erişilebilir |
| 2.4.6 Headings and Labels | Başlıklar içeriği açıklıyor, label'lar anlamlı |
| 3.3.1 Error Identification | Form hata mesajları `aria-describedby` ile bağlı |
| 4.1.2 Name, Role, Value | Icon-only butonlarda `aria-label` zorunlu |
| Canlı bölgeler | Dinamik içerik için `aria-live="polite"` veya `role="status"` |
| Navigasyon ayrımı | Birden fazla `<nav>` varsa her birinde `aria-label` |

**Kontrol yöntemi:**
```
# Alt attribute eksikliği
grep -n "<img" dosya.html | grep -v "alt="

# Icon-only buton kontrolü (aria-label olmayan)
grep -n "<button" dosya.html  # Ardından içerik incele

# aria-describedby bağlantısı
grep -n "aria-describedby\|invalid-feedback" dosya.html
```

### 3. Pattern Tutarlılığı — Önemli (P1)

Mevcut wireframe dosyalarıyla karşılaştır:

| Pattern | Referans Dosya | Kontrol |
|---|---|---|
| Breadcrumb yapısı | `vehicle-option-form.html` | `<nav aria-label="breadcrumb">` + `<ol class="breadcrumb">` |
| Card düzeni | `vehicle-list-page.html` | `<div class="card shadow-sm">` |
| Filtre formu konumu | `vehicle-list-page.html` | Tablo üstünde, card içinde |
| Sayfalama | `vehicle-list-page.html` | `card-footer` içinde, sağ hizalı |
| Bootstrap CDN | Tüm wireframe'ler | `bootstrap@5.3.0` sürümü |
| Bootstrap Icons CDN | Tüm wireframe'ler | `bootstrap-icons@1.11.0` sürümü |
| `wireframe-note` class | Tüm wireframe'ler | Placeholder notlar için sarı border-dashed kutu |
| Body arka plan | Tüm wireframe'ler | `body { background: #f8f9fa; }` |

### 4. User Story Kapsam Kontrolü — Önemli (P1)

Her Acceptance Criteria maddesini wireframe ile çapraz kontrol et:

```
Kontrol akışı:
1. US-XXX dosyasını aç
2. Her AC maddesini oku
3. Wireframe'de karşılığını bul
4. Bulunmayanları "Eksik Kapsam" listesine ekle
```

**Özel kontroller:**

- **Liste sayfaları**: Filtre alanları, tablo sütunları, sayfalama, boş durum → hepsi mevcut mu?
- **Form sayfaları**: Tüm zorunlu alanlar, validation mesajları, submit/cancel butonları → var mı?
- **Dashboard**: İstatistik kartları, eylem butonları, son aktivite listesi → kapsanıyor mu?

### 5. Responsive Tasarım — Önerilir (P2)

```html
<!-- ✅ Doğru: Mobil-first grid -->
<div class="col-12 col-md-6 col-lg-4">

<!-- ❌ Uyarı: Sadece büyük ekran tanımı -->
<div class="col-lg-4">
```

Kontrol adımları:
1. `col-` sınıflarında `col-12` veya `col-sm-*` başlangıç değeri var mı?
2. Tablo için `<div class="table-responsive">` sarmalayıcısı kullanılmış mı?
3. Mobil'de kırılabilecek uzun içerik alanları belirlenmiş mi?

## Bulgu Raporlama

Denetim sonucunda bulgular şu formatta raporlanır:

```
## Wireframe Denetim Raporu: [dosya-adı].html
Tarih: [YYYY-MM-DD]
İlişkili User Story: US-XXX

### ❌ Kritik Sorunlar (P0) — Wireframe kabul edilemez
- [Sorun açıklaması] — Satır: [N]
  ↳ Düzeltme: [Yapılması gereken]

### ⚠️ Önemli Sorunlar (P1) — Düzeltme gerekli
- [Sorun açıklaması] — Satır: [N]
  ↳ Düzeltme: [Yapılması gereken]

### 💡 Öneriler (P2) — İyileştirme fırsatı
- [Öneri açıklaması]

### ✅ Kapsam Kontrolü
- [x] AC-1: [Başlık] — Karşılandı
- [x] AC-2: [Başlık] — Karşılandı
- [ ] AC-3: [Başlık] — **EKSİK**: [Açıklama]

### Sonuç
[KABUL / KOŞULLU KABUL / RED]
Açıklama: [Özet]
```

## Onay Kriterleri

| Durum | Koşul |
|---|---|
| **KABUL** | P0 sorun yok, tüm AC maddeleri karşılandı |
| **KOŞULLU KABUL** | P0 sorun yok, ancak P1 sorunlar veya eksik AC ≤ 1 adet |
| **RED** | Herhangi bir P0 sorunu var VEYA 2+ AC maddesi eksik |

## Kontrol Listesi
- [ ] Semantic HTML kuralları (P0 listesi) denetlendi
- [ ] WCAG 2.1 AA kriterleri kontrol edildi
- [ ] Heading hiyerarşisi doğru (h1→h2→h3)
- [ ] Tüm form elemanlarında label-id eşleşmesi var
- [ ] Icon-only butonlarda `aria-label` mevcut
- [ ] Bootstrap 5.3.0 ve Bootstrap Icons 1.11.0 CDN referansları doğru
- [ ] Mevcut wireframe'lerle pattern tutarlılığı kontrol edildi
- [ ] User story'deki tüm AC maddeleri çapraz kontrol edildi
- [ ] Tablo sarmalayıcısı `table-responsive` mevcut (varsa)
- [ ] Bulgu raporu oluşturuldu ve önceliklendirildi
