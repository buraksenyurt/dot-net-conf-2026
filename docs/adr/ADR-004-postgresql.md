# ADR-004: Birincil Veritabanı — PostgreSQL

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy sistemde Microsoft SQL Server kullanılıyordu. Modernizasyon kapsamında PoC için veritabanı teknolojisi yeniden değerlendirildi. Temel gereksinimler:

- Üretim kalitesinde ilişkisel veri yönetimi
- JSON ve yarı yapılandırılmış veri desteği (araç özellikleri, opsiyonlar)
- Gelişmiş indeksleme (GIN, BRIN) ve metin arama
- Açık kaynak ve maliyet avantajı (lisans maliyetsiz)
- Docker ve bulut ortamlarıyla uyumluluk
- Windows dışı işletim sistemlerinde çalışabilirlik (geliştirici makinesi bağımsızlığı)

## Değerlendirilen Seçenekler

- **Seçenek A — Microsoft SQL Server**: Mevcut ekip bilgisi yüksek, legacy ile uyumlu. Ancak lisans maliyeti yüksek, Linux desteği kısıtlı (Express ücretsiz ama limitleri var).
- **Seçenek B — PostgreSQL**: Açık kaynak, gelişmiş özellikler (JSONB, CTE, window functions, full-text search), Docker dostu, EF Core ve Dapper ile tam uyumlu.
- **Seçenek C — MySQL / MariaDB**: Açık kaynak, yaygın. Ancak PostgreSQL'e kıyasla daha az gelişmiş tip sistemi ve kısıt desteği.
- **Seçenek D — SQLite**: Geliştirme için ideal ama üretim için yetersiz; dağıtık ve yüksek eşzamanlılık gerektiren DMS senaryoları için uygun değil.

## Karar

**PostgreSQL 16** seçildi.

Docker Compose yapılandırması:
```yaml
postgres:
  image: postgres:16
  environment:
    POSTGRES_DB: vehicle_inventory
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
  ports:
    - "5432:5432"
```

PgAdmin 4 de geliştirme ortamına eklendi (görsel yönetim aracı).

Bu tercih yapıldı çünkü:
- Sıfır lisans maliyeti; PoC bütçesiyle uyumlu
- EF Core `Npgsql.EntityFrameworkCore.PostgreSQL` sağlayıcısı olgun ve aktif
- JSONB: ileride araç özelliklerini esnek depolamak için kapı açık kalır
- Full-text search: araç listesi ve müşteri arama için hazır altyapı
- Tüm geliştiriciler Docker ile aynı ortamda çalışır; "bende çalışıyor" sorunu yoktur

## Sonuçlar

### Olumlu Sonuçlar

- Lisans maliyeti sıfır; üretim ortamına geçişte de geçerli
- Docker Compose ile geliştirici ortamı saniyeler içinde ayağa kalkar
- EF Core migration'ları PostgreSQL'e özgü tipler (UUID, JSONB, `timestamp with time zone`) kullanabilir
- Güçlü ACID garantileri; DMS için kritik finansal işlemlerde güvenilir

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Ekibin legacy MSSQL bilgisi PostgreSQL'e taşınırken öğrenme maliyeti var (sözdizimi farkları, tool farklılıkları)
- MSSQL'e özgü legacy stored procedure'lar doğrudan taşınamaz; yeniden yazılmalı
- Windows Authentication gibi kurumsal MSSQL özellikleri mevcut değil (Keycloak bu boşluğu dolduruyor)

## İlgili ADR'ler

- [ADR-005](ADR-005-ef-core-dapper.md) — EF Core + Dapper (PostgreSQL üzerindeki veri erişim stratejisi)
- [ADR-006](ADR-006-keycloak.md) — Keycloak (Windows Auth yerine kullanılan kimlik doğrulama)
