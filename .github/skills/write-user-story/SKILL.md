---
name: write-user-story
description: Bayi Yönetim Sistemi (DMS) için kullanıcı hikayesi (user story) yazma. INVEST prensipleri, kabul kriterleri (AC), iş kuralları (BR), test senaryoları ve docs/business/ şablonuna uygun US-XXX formatında dokümantasyon üretir. Domain terminolojisi ve Ubiquitous Language'ı korur.
---

# Skill: Kullanıcı Hikayesi (User Story) Yazma

## Ne Zaman Kullanılır
- Yeni bir özellik için gereksinim dokümantasyonu hazırlanacaksa
- Mevcut bir user story güncellenmesi veya genişletilmesi gerekiyorsa
- Acceptance Criteria belirsiz ya da eksikse
- Domain terminolojisi netleştirilmesi gerekiyorsa

## Gerekli Context Dosyaları
- `docs/business/_template-user-story.md` — User story şablonu
- `docs/domain-model/` — İlgili entity ve value object'ler
- `docs/business/US-*.md` — Referans user story örnekleri

## User Story Dosya Konumu

```
docs/business/US-[XXX]-[kisa-baslik].md
```

## User Story Şablonu

```markdown
# US-[XXX]: [Kısa Başlık]

## Kullanıcı Hikayesi
**[Kullanıcı rolü] olarak**,  
**[Yapılacak işlem] istiyorum**,  
**Böylece [İş değeri / amaç]**.

## Bağlam ve Amaç
[2-3 cümlelik bağlam açıklaması. Neden bu özellik gerekli?]

## Kabul Kriterleri

### AC-1: [Kriter Başlığı]
- [ ] Spesifik gereksinim 1
- [ ] Spesifik gereksinim 2
- [ ] Spesifik gereksinim 3

### AC-2: [Kriter Başlığı]
- [ ] Spesifik gereksinim 1
- [ ] Spesifik gereksinim 2

## İş Kuralları

### BR-1: [Kural Başlığı]
[Kural açıklaması. Spesifik, ölçülebilir ve test edilebilir olmalı.]

### BR-2: [Kural Başlığı]
[Kural açıklaması]

## Kapsam Dışı (Out of Scope)
- [Bu story'de yapılmayacak şeyler]

## Bağımlılıklar
- [Önce tamamlanması gereken US veya teknik gereksinimler]

## Test Senaryoları

### Senaryo 1: [Happy Path Adı]
**Koşul**: [Başlangıç durumu]  
**Adımlar**:  
1. [Adım 1]
2. [Adım 2]  
**Beklenen sonuç**: [Ne olmalı]

### Senaryo 2: [Hata Senaryosu Adı]
**Koşul**: [Hatalı giriş veya geçersiz durum]  
**Adımlar**:  
1. [Adım 1]  
**Beklenen sonuç**: [Hata mesajı veya davranış]

## UI Referansı
- `docs/ui/[feature-page].html` — Wireframe referansı (varsa)

## Domain Referansları
- Entity: `docs/domain-model/entity-[name].md`
- Value Object: `docs/domain-model/value-object-[name].md`

## Tahmini Büyüklük
[ ] XS (< 2 saat) [ ] S (yarım gün) [ ] M (1 gün) [ ] L (2-3 gün) [ ] XL (1 hafta+)
```

## INVEST Prensipleri Kontrol Listesi

| Prensip | Kontrol |
|---------|---------|
| **I**ndependent | Diğer story'lerden bağımsız teslim edilebilir mi? |
| **N**egotiable | Detaylar müzakere edilebilir, kural değil |
| **V**aluable | Kullanıcıya veya iş sürecine net değer katıyor mu? |
| **E**stimable | Geliştirici tahmin yapabilir mi? |
| **S**mall | 1 sprint'te tamamlanabilir mi? |
| **T**estable | Acceptance criteria test edilebilir mi? |

## Domain Terminolojisi (Ubiquitous Language)

Aşağıdaki Türkçe-İngilizce terminolojiyi user story'lerde tutarlı kullan:

| Türkçe | İngilizce (Kod) |
|--------|-----------------|
| Araç | Vehicle |
| Şase No | VIN |
| Bayi | Dealer |
| Müşteri | Customer |
| Servis Danışmanı | ServiceAdvisor |
| Envanter | Inventory |
| Durum | Status |
| Satın alma fiyatı | PurchasePrice (Money) |
| Önerilen satış fiyatı | SuggestedPrice (Money) |

## Yazım Kuralları
- User story cümlesini **kalın** yaz
- Kabul kriterleri checkbox listesi olmalı: `- [ ]`
- İş kuralları numaralandırılmış (BR-1, BR-2, ...)
- Test senaryoları Given/When/Then mantığıyla yaz
- Kapsam dışı maddeler açıkça belirtilmeli
