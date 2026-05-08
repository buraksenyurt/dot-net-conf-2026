---
name: create-wireframe
description: User story gereksinimlerinden semantic HTML5 + Bootstrap 5 wireframe oluşturma. docs/ui/ klasörüne .html dosyası üretir; heading hiyerarşisi, ARIA etiketleri, erişilebilirlik ve mevcut wireframe pattern tutarlılığını sağlar.
---

# Skill: HTML Wireframe Oluşturma

## Ne Zaman Kullanılır
- Yeni bir user story için UI prototipi hazırlanacaksa
- `docs/ui/` altında henüz wireframe yoksa
- Frontend geliştirme öncesinde "source of truth" HTML oluşturulacaksa
- Birden fazla ekrana sahip bir akış için wireframe seti üretilecekse

## Gerekli Context Dosyaları
- `docs/business/US-XXX-*.md` — User Story, Acceptance Criteria, iş kuralları
- `docs/ui/` — Mevcut wireframe'ler (pattern tutarlılığı için)
- `docs/domain-model/entity-*.md` — Gösterilecek veri alanları

## Çıktı Konumu

```
docs/ui/
└── [feature-kisa-adi].html   # Wireframe dosyası
```

Birden fazla ekran gerekiyorsa her ekrana ayrı dosya:
```
docs/ui/
├── [feature]-list.html
├── [feature]-form.html
└── [feature]-detail.html
```

## Adım Adım Süreç

### 1. User Story Analizi
- Acceptance Criteria'daki her ekran bileşenini listele
- Hangi veri alanları gösterilecek? (entity'lerden çek)
- Kullanıcı hangi aksiyonları gerçekleştiriyor? (butonlar, formlar)
- Filtreleme / sayfalama / sıralama gereksinimleri var mı?
- Boş durum (empty state) ve hata durumları gerekiyor mu?

### 2. Mevcut Wireframe'leri İncele
- `docs/ui/` altındaki HTML dosyalarını oku
- Breadcrumb, card layout, tablo yapısı, form düzeni gibi yerleşik pattern'leri al
- Bootstrap 5 class kullanım tutarlılığını koru
- `wireframe-note` CSS sınıfını placeholder notlar için kullan

### 3. Wireframe Dosyası Oluştur

**Temel HTML Şablonu:**
```html
<!DOCTYPE html>
<html lang="tr">
<head>
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>[Ekran Adı] — Wireframe</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.min.css" />
    <style>
        body { background: #f8f9fa; }
        /* Wireframe yer tutucu notlar için */
        .wireframe-note {
            border: 1px dashed #adb5bd;
            background: #fff3cd;
            font-size: 0.8rem;
            padding: 4px 8px;
            border-radius: 4px;
        }
    </style>
</head>
<body>
<div class="container py-5">

    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="#">Anasayfa</a></li>
            <li class="breadcrumb-item active" aria-current="page">[Ekran Adı]</li>
        </ol>
    </nav>

    <h1 class="h3 mb-1">[Ekran Başlığı]</h1>
    <p class="text-muted mb-4">[Kısa açıklama]</p>

    <!-- İçerik buraya -->

</div>
</body>
</html>
```

### 4. Bileşen Şablonları

**Liste / Tablo (AC'de listeleme varsa):**
```html
<section aria-labelledby="list-heading">
    <h2 id="list-heading" class="visually-hidden">Kayıt Listesi</h2>

    <!-- Filtre Alanı -->
    <div class="card shadow-sm mb-3">
        <div class="card-body">
            <form role="search" aria-label="Filtreler">
                <div class="row g-2 align-items-end">
                    <div class="col-md-4">
                        <label for="search-input" class="form-label">Arama</label>
                        <input type="search" id="search-input" class="form-control"
                               placeholder="Ad, soyad…" aria-describedby="search-hint" />
                        <div id="search-hint" class="form-text wireframe-note">
                            Kısmi eşleşme — büyük/küçük harf duyarsız
                        </div>
                    </div>
                    <div class="col-auto">
                        <button type="submit" class="btn btn-primary">
                            <i class="bi bi-search me-1" aria-hidden="true"></i>Ara
                        </button>
                        <button type="reset" class="btn btn-outline-secondary ms-1">Temizle</button>
                    </div>
                </div>
            </form>
        </div>
    </div>

    <!-- Tablo -->
    <div class="card shadow-sm">
        <div class="table-responsive">
            <table class="table table-hover mb-0" aria-label="[Varlık] listesi">
                <thead class="table-light">
                    <tr>
                        <th scope="col">[Sütun 1]</th>
                        <th scope="col">[Sütun 2]</th>
                        <th scope="col" class="text-end">İşlemler</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>[Örnek Veri 1]</td>
                        <td>[Örnek Veri 2]</td>
                        <td class="text-end">
                            <button class="btn btn-sm btn-outline-primary" aria-label="[Varlık] detayı">
                                <i class="bi bi-eye" aria-hidden="true"></i>
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Sayfalama -->
        <div class="card-footer d-flex justify-content-between align-items-center">
            <small class="text-muted">Toplam <strong>45</strong> kayıt</small>
            <nav aria-label="Sayfalama">
                <ul class="pagination pagination-sm mb-0">
                    <li class="page-item disabled"><a class="page-link" href="#">«</a></li>
                    <li class="page-item active"><a class="page-link" href="#">1</a></li>
                    <li class="page-item"><a class="page-link" href="#">2</a></li>
                    <li class="page-item"><a class="page-link" href="#">3</a></li>
                    <li class="page-item"><a class="page-link" href="#">»</a></li>
                </ul>
            </nav>
        </div>
    </div>

    <!-- Boş Durum -->
    <div class="text-center py-5 text-muted d-none" role="status" aria-live="polite">
        <i class="bi bi-inbox fs-1 d-block mb-2" aria-hidden="true"></i>
        <p class="mb-0">Kayıt bulunamadı.</p>
    </div>
</section>
```

**Form (AC'de veri girişi varsa):**
```html
<section aria-labelledby="form-heading">
    <h2 id="form-heading" class="visually-hidden">Kayıt Formu</h2>
    <div class="card shadow-sm">
        <div class="card-header bg-white fw-semibold">[Form Başlığı]</div>
        <div class="card-body">
            <form novalidate aria-label="[İşlem] formu">

                <div class="mb-3">
                    <label for="field-id" class="form-label">
                        [Alan Adı] <span class="text-danger" aria-label="zorunlu">*</span>
                    </label>
                    <input type="text" id="field-id" name="fieldName"
                           class="form-control" required
                           aria-describedby="field-id-error" />
                    <div id="field-id-error" class="invalid-feedback">
                        Bu alan zorunludur.
                    </div>
                </div>

                <!-- Hata mesajı bandı -->
                <div class="alert alert-danger d-none" role="alert" aria-live="assertive">
                    İşlem sırasında bir hata oluştu. Lütfen tekrar deneyin.
                </div>

                <div class="d-flex gap-2">
                    <button type="submit" class="btn btn-primary">
                        <span class="spinner-border spinner-border-sm me-2 d-none"
                              role="status" aria-hidden="true"></span>
                        Kaydet
                    </button>
                    <a href="#" class="btn btn-outline-secondary">İptal</a>
                </div>
            </form>
        </div>
    </div>
</section>
```

**Durum Badge'leri (AC'de status gösterimi varsa):**
```html
<!-- Status badge — renk iş kurallarına göre ayarla -->
<span class="badge bg-success">Aktif</span>
<span class="badge bg-warning text-dark">Süresi Dolmuş</span>
<span class="badge bg-secondary">İptal Edilmiş</span>
```

### 5. Erişilebilirlik Zorunlulukları

Her wireframe'de şunlar **mutlaka** bulunmalı:
- `<html lang="tr">` — dil tanımı
- Tüm `<img>` etiketlerinde `alt` attribute
- Her `<input>` için eşleşen `<label for="...">` veya `aria-label`
- `<table>` başlıklarında `scope="col"` veya `scope="row"`
- `<nav aria-label="...">` ile çoklu navigasyon ayrımı
- Dinamik bölgelerde `role="status"` veya `aria-live`
- `<button>` icon-only ise `aria-label` zorunlu

### 6. Mobil-First Responsive Kural

```html
<!-- ✅ Doğru: col sınıfları küçükten büyüğe -->
<div class="col-12 col-md-6 col-lg-4">

<!-- ❌ Yanlış: sadece büyük ekran tanımı -->
<div class="col-lg-4">
```

## Kontrol Listesi
- [ ] User story'deki her AC maddesi wireframe'de temsil ediliyor
- [ ] Heading hiyerarşisi doğru (h1 → h2 → h3, atlama yok)
- [ ] Tüm form alanlarında `<label>` ve `id` eşleşmesi var
- [ ] Tablo için `<table>` kullanıldı, layout için değil
- [ ] `<button>` aksiyonlar için, `<a>` navigasyon için kullanıldı
- [ ] Boş durum (empty state) gösterimi mevcut
- [ ] Hata durumu (error state) ve yükleme (loading) gösterimi mevcut
- [ ] Bootstrap 5 class'ları mevcut wireframe'lerle tutarlı
- [ ] `wireframe-note` sınıfıyla açıklayıcı notlar eklendi
- [ ] Mobil-first `col-12 col-md-X` grid düzeni uygulandı
- [ ] `lang="tr"` ve `charset="UTF-8"` mevcut
