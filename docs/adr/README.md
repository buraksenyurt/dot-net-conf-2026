# Architectural Decision Records (ADR)

Bu klasör, proje kapsamında alınan önemli mimari kararları [ADR formatında](https://adr.github.io/) belgeler.

## ADR Nedir?

Architectural Decision Record (Mimari Karar Kaydı), yazılım mimarisini etkileyen önemli bir kararı, bu kararın bağlamını ve sonuçlarını kısa ve yapılandırılmış bir biçimde belgeleyen bir döküman türüdür.

## ADR Durumları

| Durum | Açıklama |
|-------|----------|
| **Taslak** | Henüz tartışma aşamasında, kesinleşmemiş |
| **Kabul Edildi** | Ekip tarafından onaylanmış ve uygulanıyor |
| **Kullanımdan Kaldırıldı** | Artık geçerli değil, yerine geçen ADR belirtilmiştir |
| **Değiştirildi** | Başka bir ADR ile revize edilmiştir |

## Karar Listesi

| # | Başlık | Durum | Tarih |
|---|--------|-------|-------|
| [ADR-001](ADR-001-clean-architecture.md) | Backend Mimari Deseni: Clean Architecture | Kabul Edildi | 2025-01-01 |
| [ADR-002](ADR-002-cqrs-mediatr.md) | Komut/Sorgu Ayrımı: CQRS ve MediatR | Kabul Edildi | 2025-01-01 |
| [ADR-003](ADR-003-domain-driven-design.md) | Domain Modelleme Yaklaşımı: DDD | Kabul Edildi | 2025-01-01 |
| [ADR-004](ADR-004-postgresql.md) | Birincil Veritabanı: PostgreSQL | Kabul Edildi | 2025-01-01 |
| [ADR-005](ADR-005-ef-core-dapper.md) | Veri Erişim Stratejisi: EF Core + Dapper | Kabul Edildi | 2025-01-01 |
| [ADR-006](ADR-006-keycloak.md) | Kimlik Doğrulama ve Yetkilendirme: Keycloak | Kabul Edildi | 2025-01-01 |
| [ADR-007](ADR-007-vue3-frontend.md) | Frontend Framework: Vue 3 + Vite | Kabul Edildi | 2025-01-01 |
| [ADR-008](ADR-008-serilog-logging.md) | Yapılandırılmış Loglama: Serilog | Kabul Edildi | 2025-01-01 |
| [ADR-009](ADR-009-fluentvalidation.md) | Girdi Doğrulama: FluentValidation | Kabul Edildi | 2025-01-01 |
| [ADR-010](ADR-010-api-versioning.md) | REST API Versiyonlama Stratejisi | Kabul Edildi | 2025-01-01 |
| [ADR-011](ADR-011-xunit-testing.md) | Backend Test Altyapısı: xUnit + Moq | Kabul Edildi | 2025-01-01 |
| [ADR-012](ADR-012-github-actions-cicd.md) | CI/CD Pipeline: GitHub Actions | Kabul Edildi | 2025-01-01 |
| [ADR-013](ADR-013-sonarqube.md) | Kod Kalitesi ve Statik Analiz: SonarQube | Kabul Edildi | 2025-01-01 |

## Yeni ADR Ekleme

Yeni bir mimari karar belgelemek için:

1. Sonraki numarayı alın (`ADR-014-...`)
2. `_template.md` şablonunu kopyalayın
3. İlgili alanları doldurun
4. Bu `README.md` dosyasındaki tabloya ekleyin
5. Pull Request açın ve ekip onayı alın
