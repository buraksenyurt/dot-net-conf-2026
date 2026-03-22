# ADR-006: Kimlik Doğrulama ve Yetkilendirme — Keycloak

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy DMS sisteminde kimlik doğrulama, Active Directory / Windows Authentication ile yapılıyordu. Modernizasyon kapsamında:

- Web tabanlı SPA'dan (Vue 3) token bazlı kimlik doğrulamaya geçilmesi gerekiyordu
- Servis danışmanı ve yönetici rol ayrımı gerektiriliyordu
- İleride mobil uygulama veya üçüncü taraf entegrasyonları için OAuth2/OIDC standardına ihtiyaç vardı
- Windows altyapısına bağımlılığı kırmak modernizasyon hedefleri arasındaydı

## Değerlendirilen Seçenekler

- **Seçenek A — ASP.NET Core Identity**: Yerleşik, kolay kurulum. Ancak tam SSO, role management ve harici identity provider entegrasyonu için ek geliştirme gerektirir.
- **Seçenek B — Keycloak**: Açık kaynak, kurumsal hazırlıklı Identity Provider. OAuth2, OIDC, SAML desteği. Docker ile kolayca ayağa kalkar.
- **Seçenek C — Auth0 / Azure AD B2C**: Bulut tabanlı, yönetilen servis. PoC için maliyet ve vendor lock-in kaygısı.
- **Seçenek D — Duende IdentityServer**: .NET ekosisteminde olgun çözüm. Ancak lisans maliyeti (ücretsiz sürümde kısıtlamalar) ve ek karmaşıklık.

## Karar

**Keycloak** seçildi.

Docker Compose ile entegre:
```yaml
keycloak:
  image: quay.io/keycloak/keycloak:latest
  environment:
    KEYCLOAK_ADMIN: admin
    KEYCLOAK_ADMIN_PASSWORD: admin
  ports:
    - "8080:8080"
  command: start-dev
```

Backend'de JWT doğrulaması:
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = "http://keycloak:8080/realms/vehicle-inventory";
        options.Audience = "vehicle-inventory-api";
    });
```

Bu tercih yapıldı çünkü:
- Açık kaynak, sıfır lisans maliyeti
- OAuth2/OIDC standardı; frontend (Vue) ile `keycloak-js` üzerinden kolay entegrasyon
- Rol ve grup yönetimi arayüzden yapılır; kod değişikliği gerektirmez
- Docker Compose ile geliştirme ortamına dahil, üretimde Kubernetes veya VM'de aynı imaj çalışır
- İleride SAML, LDAP veya sosyal giriş (Google, Microsoft) eklenebilir

## Sonuçlar

### Olumlu Sonuçlar

- Kimlik yönetimi (kayıt, şifre sıfırlama, MFA) Keycloak admin console'dan yönetilir; backend kodu sade kalır
- JWT token'lar stateless; API gateway veya microservice geçişine hazır
- Geliştirici makinesi tam yerel çalışır; bulut bağımlılığı yok
- Standart OIDC claim'leri (`sub`, `roles`, `email`) tüm sistemde tutarlı

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Keycloak Java tabanlı, görece ağır; geliştirici makinesinde ~512 MB RAM tüketir
- Realm ve client konfigürasyonu ilk kurulumda manuel yapılmalı (IaC ile otomasyona açık)
- `start-dev` modu üretimde kullanılmamalı; üretim konfigürasyonu ek adım gerektirir

## İlgili ADR'ler

- [ADR-007](ADR-007-vue3-frontend.md) — Vue 3 Frontend (Keycloak JS entegrasyonu)
- [ADR-004](ADR-004-postgresql.md) — PostgreSQL (Keycloak kendi veritabanını kullanır; ayrı instance önerilir)
