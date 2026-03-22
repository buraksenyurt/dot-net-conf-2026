# ADR-007: Frontend Framework — Vue 3 + Vite + TypeScript

## Durum

Kabul Edildi

## Tarih

2025-01-01

## Bağlam

Legacy DMS sisteminde frontend, ASP.NET Web Forms üzerine kuruluydu; sunucu taraflı render, ViewState ve UpdatePanel kullanılıyordu. Bu yaklaşımın modern bir SPA'ya geçişte karşılaşılan sorunları:

- Kullanıcı deneyimi yetersizdi; her işlem sayfayı yeniliyordu
- Frontend ve backend arasındaki sıkı bağlılık bağımsız dağıtımı engelliyordu
- TypeScript desteği yoktu; büyük frontend kodta tip güvenliği sağlanamıyordu
- Modern bileşen tabanlı geliştirme mümkün değildi

Seçilecek framework'ün şu kriterleri karşılaması beklendi:
- Hızlı geliştirme (PoC bütçesi)
- Kapsamlı ekosistem ve topluluk
- TypeScript first desteği
- Bootstrap 5 ile uyumluluk (kurumsal UI kitleriyle uyum)
- AI destekli kod üretiminde iyi kapsam

## Değerlendirilen Seçenekler

- **Seçenek A — React**: En geniş ekosistem, JSX, büyük topluluk. Ancak boilerplate fazla, opinionated değil (state management, routing kararı ayrıca alınmalı). AI desteği mükemmel.
- **Seçenek B — Vue 3 Composition API**: Sezgisel öğrenme eğrisi, Single File Component (SFC), güçlü TypeScript desteği, Vite ile birlikte çok hızlı. Ekip deneyimi mevcut.
- **Seçenek C — Angular**: Kurumsal, opinionated. Ancak ağır boilerplate, dik öğrenme eğrisi; PoC hızı için fazla ağır.
- **Seçenek D — Blazor (WebAssembly)**: .NET ekibi için doğal geçiş. Ancak ekosistemin görece genç olması ve WASM performans başlangıç sorunları risk oluşturuyor.

## Karar

**Vue 3 Composition API + Vite + TypeScript** seçildi. UI kitaplığı olarak **Bootstrap 5** kullanıldı.

Paket yöneticisi olarak **Yarn** tercih edildi (Windows native binding uyumluluğu).

Proje yapısı şu prensiplere dayanıyor:
- **Atomic Design**: `atoms/`, `molecules/`, `organisms/` bileşen hiyerarşisi
- **Composables**: veri getirme ve durum yönetimi `use*.ts` dosyalarında
- **PascalCase** bileşen isimlendirmesi
- **Single File Component**: template, script ve style aynı `.vue` dosyasında

Vite konfigürasyonu: Vue eklentisi ve ödev için proxy (`/api` → backend)

Bu tercih yapıldı çünkü:
- Ekip Vue 3 deneyimine sahip; öğrenme maliyeti düşük
- Vite, Webpack'e kıyasla çok daha hızlı HMR ve build süresi — PoC sürecinde kritik
- `<script setup>` syntax'ı bileşen geliştirmeyi önemli ölçüde sadeleştirir
- Bootstrap 5 kurumsal görünümü sağlarken ekstra tasarım kararı almayı önler
- AI araçları (Copilot, Claude) Vue 3 SFC ve Composition API'yi iyi biliyor; kod üretim kalitesi yüksek

## Sonuçlar

### Olumlu Sonuçlar

- Vite ile geliştirme sunucusu anında başlar; HMR neredeyse anlık
- TypeScript + Vue 3 tip güvenliği komplex form ve API entegrasyonlarında hata oranını düşürür
- Bootstrap 5 bileşenleri hızlı UI prototipi sağlar
- Composables ile veri getirme mantığı bileşenlerden ayrılır; test edilebilirlik artar
- `docs/ui/` klasöründeki HTML wireframe'ler doğrudan Vue bileşenlerine dönüştürülür

### Olumsuz Sonuçlar / Kabul Edilen Ödünleşimler

- Yarn kullanımı; npm alışkanlığı olan geliştiriciler için küçük alışma süreci
- Vue ekosisteminde state management (Pinia) ayrıca kurulmalı; React gibi tek bir "standart" yoktur
- Bootstrap 5 ve Vue arasındaki reaktif bileşen entegrasyonu (modaller, tooltips) dikkat gerektirir (Bootstrap'ın kendi JS'i yerine Vue bileşeni kullanılmalı)

## İlgili ADR'ler

- [ADR-006](ADR-006-keycloak.md) — Keycloak (frontend'de `keycloak-js` entegrasyonu)
- [ADR-010](ADR-010-api-versioning.md) — API versiyonlama (frontend'in API URL'leri nasıl tükettiği)
