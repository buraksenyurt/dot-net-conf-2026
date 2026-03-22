# RAG Örnek Sorguları

Bu dosya, `docs_knowledge_base` koleksiyonuna yüklenmiş dokümanlar üzerinde
semantik arama & RAG düzeneğiyle test edilebilecek örnek sorguları içerir.

---

## Kategori 1 — Mimari Kararlar (`adr/`)

1. **"Clean Architecture neden seçildi, alternatifler neydi?"**  
   → ADR-001'in "Değerlendirilen Seçenekler" ve "Karar" bölümleri getirilir.

2. **"CQRS ve MediatR kullanmanın kabul edilen ödünleşimleri neler?"**  
   → ADR-002'nin "Olumsuz Sonuçlar" bölümü getirilir.

3. **"Keycloak yerine başka bir authentication çözümü neden tercih edilmedi?"**  
   → ADR-006'nın gerekçe bölümü getirilir.

---

## Kategori 2 — İş Gereksinimleri (`business/`)

4. **"Araç stoğa eklenirken hangi validasyonlar uygulanıyor?"**  
   → US-001'in "Kabul Kriterleri - AC-4 Validasyonlar" bölümü getirilir.

5. **"Bir müşteri kaydı oluştururken zorunlu alanlar neler?"**  
   → US-004 veya US-006'nın kabul kriterleri bölümü getirilir.

---

## Kategori 3 — Domain Model (`domain-model/`)

6. **"Vehicle entity'sindeki Money value object nasıl çalışır, para birimi kısıtlamaları var mı?"**  
   → `entity-vehicle.md` fiyat bilgileri bölümü + `value-object-money.md` birlikte getirilir.

7. **"VIN numarası hangi kurallara göre doğrulanıyor?"**  
   → `value-object-vin.md` ve US-001'in "BR-1: VIN Doğrulama" bölümü getirilir.

---

## Kategori 4 — Teknoloji Altyapısı (`architectural-overview/`)

8. **"Frontend'de state management için ne kullanılıyor, neden ayrı bir kütüphane yok?"**  
   → `01-technology-stack.md`'nin "Vue Composition API" bölümü getirilir.

9. **"Loglama için Serilog mu OpenTelemetry mi tercih edilmeli, ikisi arasındaki fark nedir?"**  
   → `01-technology-stack.md`'nin "Logging ve Monitoring" bölümü getirilir.

---

## Kategori 5 — Genel / Çapraz Kategori

10. **"Yeni bir API endpoint'i oluştururken hangi katmanlara dokunmam gerekiyor ve sırasıyla ne yapmalıyım?"**  
    → `prompts/01-create-api-endpoint.md` + ADR-001 Clean Architecture katman tanımları + `02-coding-standards.md` bir arada getirilir.

---

## Kategori 6 — Test & Kalite (`adr/` + `architectural-overview/`)

11. **"Domain katmanı için birim testi yazarken hangi test framework'ünü kullanmalıyım?"**  
    → ADR-011 (xUnit) ve `02-coding-standards.md`'nin test bölümü getirilir.

12. **"SonarQube hangi amaçla entegre edildi, hangi kalite kapısı kuralları uygulanıyor?"**  
    → ADR-013'ün gerekçe ve sonuçlar bölümü getirilir.

---

## Kategori 7 — Veri Erişimi (`adr/`)

13. **"Entity Framework Core ile Dapper'ı aynı projede kullanmanın gerekçesi nedir?"**  
    → ADR-005'in karar ve bağlam bölümü getirilir.

14. **"PostgreSQL'e geçişte SQL Server'a kıyasla ne gibi ödünleşimler kabul edildi?"**  
    → ADR-004'ün değerlendirilen seçenekler bölümü getirilir.

---

## Kategori 8 — Geliştirici Deneyimi & Araçlar (`prompts/` + `architectural-overview/`)

15. **"EF Core migration oluştururken hangi komutları çalıştırmam gerekiyor?"**  
    → `prompts/02-create-ef-migration.md` getirilir.

16. **"Yeni bir Vue bileşeni eklerken proje yapısında dosyayı nereye koymalıyım?"**  
    → `prompts/03-create-vue-component.md` + `03-project-structure.md` getirilir.

---

## Kategori 9 — Güvenlik & Kimlik (`adr/`)

17. **"JWT token doğrulaması API katmanında nasıl yapılandırılıyor?"**  
    → ADR-006 Keycloak'ın "JWT Bearer Token" bölümü getirilir.

18. **"Kullanıcı rolleri ve yetkilendirme politikaları nerede tanımlanıyor?"**  
    → ADR-006 ile `02-coding-standards.md`'nin güvenlik bölümü getirilir.

---

## Kategori 10 — Statik Veriler & Enum'lar (`static-data/`)

19. **"Sistemde tanımlı araç markaları ve modeller nereden geliyor, dinamik mi statik mi?"**  
    → `static-data/brands-and-models.md` getirilir.

20. **"VehicleStatus enum'undaki durum geçişleri iş kurallarıyla nasıl ilişkilendirilmiş?"**  
    → `static-data/enums.md` + `domain-model/entity-vehicle.md`'nin "Business Logic Methods" bölümü birlikte getirilir.

---

## Retrieval Kalitesi Hakkında Not

Her chunk'ın `EmbeddingText`'i şu formatla zenginleştirilmiştir:

```
Kaynak: {SourceFile} | Kategori: {Category}
Doküman: {DocumentTitle} > {SectionPath}

{Content}
```

Bu sayede:
- **Kategori filtresi** uygulanabilir (sadece `adr/` veya sadece `business/` chunk'larında ara).
- **Çapraz kategori** sorguları (örn. 10, 20) birden fazla kaynaktan semantik olarak ilgili bölümleri bir arada getirir.
- Sorgu metnine `"neden"`, `"gerekçe"`, `"ödünleşim"` gibi kelimeler eklemek ADR'lerin karar bölümlerini, `"nasıl"` ve `"adım"` eklemek ise prompt/kılavuz chunk'larını öne çıkarır.
