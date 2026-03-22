# ADR-012: CI/CD Pipeline — GitHub Actions

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy sistemde CI/CD, Azure DevOps Pipelines üzerinde yönetiliyordu. PoC projesinin kaynak kodu GitHub'da barındırıldığından, CI/CD altyapısı da GitHub ekosistemine taşındı.

Gereksinimler:
- Her pull request'te otomatik build ve test
- Kod kalitesi analizi (SonarQube entegrasyonu)
- Backend (.NET) ve frontend (Vue + TypeScript) için ayrı iş akışları
- Docker imajı build ve push (ileride)
- Kolay konfigürasyon ve bakım

## Değerlendirilen Seçenekler

- **Seçenek A — GitHub Actions**: GitHub ile yerleşik entegrasyon, YAML tabanlı, geniş marketplace, ücretsiz tier yeterli. Depo içinde `.github/workflows/` altında versiyonlanır.
- **Seçenek B — Azure DevOps Pipelines**: Ekibin mevcut deneyimi var. Ancak kaynak kodu GitHub'da olduğunda ek yapılandırma gerektirir; iki platform arası geçiş sürtünmesi vardır.
- **Seçenek C — GitLab CI/CD**: Güçlü ama kaynak kodu GitHub'da; platform değişikliği gerekirdi.
- **Seçenek D — Jenkins**: Self-hosted, tam kontrol. Ancak altyapı bakımı ve konfigürasyon karmaşıklığı PoC için aşırı.

## Karar

**GitHub Actions** seçildi. Git Flow branching stratejisi benimsendi:

```
main        → production-ready
develop     → entegrasyon dalı
feature/*   → özellik geliştirme
hotfix/*    → acil düzeltme
```

CI iş akışı tetikleyicileri:
- `pull_request` → `develop` veya `main` dallarına

Backend pipeline adımları:
1. .NET 9 kurulumu
2. `dotnet restore`
3. `dotnet build --no-restore`
4. `dotnet test --collect:"XPlat Code Coverage"`
5. SonarQube analiz (coverage raporu ile)

Frontend pipeline adımları:
1. Node.js kurulumu
2. `yarn install`
3. `yarn build`
4. `yarn test:unit` (Vitest)

Conventional Commits zorunlu kılındı:
- `feat:` → yeni özellik
- `fix:` → hata düzeltme
- `docs:` → dokümantasyon
- `test:` → test ekleme/güncelleme
- `refactor:` → yeniden yapılandırma
- `chore:` → araç/bağımlılık güncelleme

Bu tercih yapıldı çünkü:
- `.github/workflows/` YAML dosyaları kaynak kodla birlikte versiyonlanır
- GitHub Marketplace'de hazır action'lar (SonarQube, Docker) kurulum süresini düşürür
- Pull request kontrolleri ile kalitesiz kod `main` dalına ulaşmadan engellenir
- Ekip GitHub'da çalıştığından platform geçişi gereksizdir

## Sonuçlar

### Olumlu Sonuçlar

- Her PR'da otomatik build/test; `main` dalı her zaman geçen testlere sahip
- SonarQube entegrasyonu PR içinde kod kalitesi kapısı sağlar
- YAML workflow'ları şeffaf; her geliştirici pipeline'ı görebilir ve değiştirebilir
- GitHub Actions dakika bazlı faturalandırma; küçük PoC için ücretsiz limit yeterli

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Self-hosted runner kurulmadığında GitHub'ın Ubuntu runner'ları kullanılır; Windows'a özgü testler için dikkat gerekir
- Sırlar (secrets) GitHub Secrets'e eklenmeli; yanlış konfigürasyon güvenlik açığı yaratabilir
- YAML sözdizimi hataları çalışma zamanında ortaya çıkar; statik doğrulama kısıtlı

## İlgili ADR'ler

- [ADR-011](ADR-011-xunit-testing.md) — xUnit Test Altyapısı (CI'da çalıştırılan testler)
- [ADR-013](ADR-013-sonarqube.md) — SonarQube (pipeline'a entegre kalite kapısı)
