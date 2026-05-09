# MCP Senaryo: DmsMcpServer

## Genel Bakış

Bu senaryo, Bayi Yönetim Sistemi (DMS) domain'i üzerine inşa edilmiş bir **Model Context Protocol (MCP) Server** uygulamasıdır. Microsoft'un resmi `ModelContextProtocol` NuGet paketi kullanılır ve workspace içindeki `MCP/DmsMcpServer/` klasöründe çalışır. Temel amaç dil modeli bazlı vekil ajanlarda aşağıdakine benzer sorular için karşılık gelen araç setinin işletilmesini sağlamaktır. Bu araç seti MCP Server tarafından kullanılan backend api noktalarını kullanır.

| **Soru** | **Beklenen Araç Çağrıları** |
| --- | --- |
| Şu an opsiyonlanabilen araçlar varsa listeler misin? | `list_vehicles` |
| Tezlaa aracı 5 günlüğüne opsiyonlamak isteyen bir müşterimiz var. Kapora olarak 10_000 Dolar yatırdı. | `list_vehicles` -> `create_option` |
| Spidi Gonzalez için bir araç opsiyonlamak istiyorum. | `list_customers` -> `list_vehicles` -> `create_option` |
| Envanterden bir Satışta aracı al, Alvo Yarnsby adlı müşteri için 7 günlük opsiyon oluştur. | `list_vehicles` -> `list_customers` -> `create_option` |

---

## Proje Yapısı

```text
MCP/
└── DmsMcpServer/
    ├── DmsMcpServer.csproj
    ├── Program.cs              ← Streamable HTTP transport, DI kayıtları
    ├── Tools/
    │   ├── VehicleTools.cs     ← list_vehicles, add_vehicle, change_vehicle_status, validate_vin
    │   ├── CustomerTools.cs    ← list_customers, register_customer
    │   ├── OptionTools.cs      ← create_option, cancel_option, get_advisor_dashboard
    │   └── DevTools.cs         ← get_user_story, get_domain_entity, list_adrs, get_adr
    └── HttpClients/
        └── DmsApiClient.cs     ← Typed HttpClient → localhost:5280
```

---

## NuGet Paketleri

```xml
<PackageReference Include="ModelContextProtocol" Version="0.3.*" />
<PackageReference Include="ModelContextProtocol.AspNetCore" Version="0.3.*" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="9.*" />
```

> `ModelContextProtocol`, Microsoft'un katkıda bulunduğu ve `modelcontextprotocol` GitHub org altında yayınlanan resmi C# SDK'dır.

---

## Transport

**`Streamable HTTP`** — Claude Desktop, VS Code Copilot veya başka bir MCP client ile doğrudan entegre olabilir. Ayrı bir port veya web sunucusu gerektirmez.

---

## Tool Kataloğu

### Sorgulama (Query) Tools

| **Tool** | **Parametreler** | **Bağlandığı Endpoint** |
| --- | --- | --- |
| `list_vehicles` | `status?`, `brand?`, `model?` | `GET /api/v1/vehicles` |
| `get_vehicle` | `vin` | `GET /api/v1/vehicles/{vin}` |
| `list_customers` | `search?`, `customerType?` | `GET /api/v1/customers` |
| `validate_vin` | `vin` | Lokal — ISO 3779 check digit algoritması (API çağrısı gerekmez) |

### Görev Tetikleme (Action) Tools

| **Tool** | **Tetiklediği Görev** | **Bağlandığı Endpoint** |
| --- | --- | --- |
| `add_vehicle` | Envantere yeni araç ekler | `POST /api/v1/vehicles` |
| `change_vehicle_status` | Araç durumunu değiştirir (`OnSale`, `InStock` vb.) | `PATCH /api/v1/vehicles/{id}/status` |
| `register_customer` | Yeni müşteri kaydı oluşturur | `POST /api/v1/customers` |
| `create_option` | Aracı müşteri adına rezerve eder | `POST /api/v1/vehicle-options` |
| `cancel_option` | Aktif opsiyonu iptal eder | `DELETE /api/v1/vehicle-options/{id}` |
| `get_advisor_dashboard` | Danışmanın tüm aktif opsiyonlarını getirir | `GET /api/service-advisors/{id}/dashboard` |

### Developer Productivity Tools

Bu araçlar, AI'ın kendi kaynaklarını okumasını sağlar — RAG altyapısı gerektirmeden.

| **Tool** | **Açıklama** | **Kaynak** |
| --- | --- | --- |
| `get_user_story` | Belirtilen US numarasının kabul kriterlerini yapılandırılmış döner | `docs/business/US-00X.md` |
| `get_domain_entity` | Domain entity tanımını döner | `docs/domain-model/entity-*.md` |
| `list_adrs` | Tüm ADR'lerin özet listesini döner | `docs/adr/` klasörü |
| `get_adr` | Belirli bir ADR'nin tam içeriğini döner | `docs/adr/ADR-00X-*.md` |

---

## Örnek Akış (Sunum Senaryosu)

> Burada özellikle sistemde kayıtlı olmayan bir müşteri bilgisi ile ilerlemek iyi olur. Zira AI müşteri ararken sonuç bulamazsa `register_customer` tool'unu tetikler ve müşteriye ait bazı bilgileri plan modunda sorar.

```text
Kullanıcı → AI:
  "Envanterden bir Satışta aracı al, Alvo Yarnsby adlı müşteri için 7 günlük opsiyon oluştur."

AI (sırasıyla tool çağrıları):
  1. list_vehicles(status="OnSale")          → [{ id, brand, model, vin, ... }]
  2. list_customers(search="Alvo Yarnsby")  → [{ id, displayName, ... }]
  3. create_option(vehicleId, customerId, validityDays=7, optionFee=0)
     → "Opsiyon oluşturuldu. Araç 'Reserved' durumuna geçti."
```

---

## Neden Bu Senaryo?

1. **Gerçek uçtan uca akış**: AI → MCP Tool → HTTP API → PostgreSQL (Docker üzerinde)
2. **Hem sorgulama hem eylem**: Doğal dil komutları hem veri okuyabilir hem işlem başlatabilir
3. **Lokal mantık örneği**: `validate_vin` tool'u API gerektirmeden domain bilgisini doğrular
4. **Developer Productivity boyutu**: `get_user_story` / `get_adr` — AI kendi bağlamını kendi kaynaklarından okur, RAG altyapısı gerekmez

---

## Geliştirme Adımları

1. `MCP/DmsMcpServer/` klasöründe `dotnet new console` ile proje oluştur
2. NuGet paketlerini ekle
3. `Program.cs` içinde `Streamable HTTP` transport ile MCP host'u kur
4. `HttpClients/DmsApiClient.cs` — Typed `HttpClient` ile backend API bağlantısını kur
5. Her `Tools/*.cs` dosyasını `[McpServerToolType]` / `[McpServerTool]` attribute'ları ile implement et
6. `VehicleInventory.slnx`'e projeyi ekle

---

## Önkoşullar

- Backend API çalışıyor olmalı: `dotnet run` → `http://localhost:5280`
- Docker servisler ayakta olmalı: `docker-compose up -d` (postgres, pgadmin)
- MCP Server çalışıyor olmalı: `dotnet run` → `http://localhost:5290`

## Inventory MCP Client

> VS Copilot arabirimi yerine local LLM kullanarak MCP server araçlarını çalıştıran deneysel örnek. Henüz geliştirme aşamasında.