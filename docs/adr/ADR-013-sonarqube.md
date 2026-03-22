# ADR-013: Kod Kalitesi ve Statik Analiz — SonarQube

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy DMS'de kod kalitesi ölçümü yapılmıyordu. Teknik borç görünmez biçimde birikti; yeni özellik eklendikçe mevcut problemler derinleşti. Modernizasyon kapsamında:

- Kod kalitesini nesnel metriklerle sürekli izlemek
- Test coverage'ı ölçmek ve %80 hedefine uyumu doğrulamak
- Güvenlik açıklarını (OWASP kategorizasyonuyla) otomatik tespit etmek
- Teknik borç birikimini görselleştirmek ve yönetmek

gereksinimler olarak belirlendi.

## Değerlendirilen Seçenekler

- **Seçenek A — SonarQube Community Edition**: Açık kaynak, self-hosted, kapsamlı kural seti. Docker ile kolayca kurulur, GitHub Actions entegrasyonu güçlü. Ücretsiz.
- **Seçenek B — SonarCloud**: SonarQube'ün bulut versiyonu. Kolay kurulum, GitHub entegrasyonu otomatik. Açık kaynak projeler için ücretsiz, özel projeler ücretli.
- **Seçenek C — Roslyn Analyzers (yerleşik)**: .NET SDK ile birlikte gelir, derleme sırasında uyarı verir. SonarQube kadar kapsamlı değil; merkezi dashboard yok.
- **Seçenek D — NDepend**: Güçlü ama ücretli, karmaşık. Büyük enterprise projeler için uygun.

## Karar

**SonarQube Community Edition** seçildi. Docker Compose ile geliştirme ortamına dahil edildi:

```yaml
sonarqube:
  image: sonarqube:community
  depends_on:
    - sonarqube_db
  ports:
    - "9000:9000"
  environment:
    SONAR_JDBC_URL: jdbc:postgresql://sonarqube_db:5432/sonarqube
    SONAR_JDBC_USERNAME: sonar
    SONAR_JDBC_PASSWORD: sonar

sonarqube_db:
  image: postgres:16
  environment:
    POSTGRES_USER: sonar
    POSTGRES_PASSWORD: sonar
    POSTGRES_DB: sonarqube
```

GitHub Actions entegrasyonu:
```yaml
- name: SonarQube Scan
  uses: SonarSource/sonarqube-scan-action@master
  env:
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
    SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
```

Kalite kapısı (Quality Gate) gereksinimleri:
- Yeni kod coverage: **%80 ve üzeri**
- Yeni kod duplications: **%3 ve altı**
- Kritik/blocker issue: **sıfır**
- Güvenlik açığı: **sıfır**

Bu tercih yapıldı çünkü:
- Self-hosted kurulum veri gizliliğini korur; kod dışarıya gönderilmez
- Docker Compose sayesinde geliştirici ortamıyla entegre; CI'da aynı instance kullanılabilir
- GitHub Actions ile PR bazlı kalite kapısı; kalitesiz kod merge edilemez
- Kapsamlı OWASP güvenlik kuralları SAST (Static Application Security Testing) sağlar
- Dashboard ile teknik borç görselleştirilir, sprint planlama sürecine dahil edilebilir

## Sonuçlar

### Olumlu Sonuçlar

- Her PR analiz edilir; sorunlar mevcut koda karışmadan engellenir
- Test coverage hedefi (%80) nesnel ölçülebilir; subjektivite ortadan kalkar
- Güvenlik açıkları kod review'dan bağımsız, otomatik tespit edilir
- Teknik borç birikmesi görünür hale gelir; kasıtlı borç kararı alınabilir

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- SonarQube başlangıçta yüksek kaynak tüketir (~2 GB RAM); geliştirici makinesinde kaynak planlaması gerekir
- İlk proje yapılandırması (token, project key, Quality Gate) elle yapılmalı
- Community Edition'da bazı enterprise özellikler (branch analizi, PR decoration tam destek) kısıtlı; SonarCloud ile tamamlanabilir
- False positive'ler için kural bastırma (suppression) yönetimi gerektirir

## İlgili ADR'ler

- [ADR-011](ADR-011-xunit-testing.md) — xUnit Test Altyapısı (coverage raporları SonarQube'e beslenir)
- [ADR-012](ADR-012-github-actions-cicd.md) — GitHub Actions (SonarQube analizi pipeline'a entegre)
