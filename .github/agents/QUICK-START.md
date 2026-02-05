# 🚀 Copilot Agent Kullanım Kılavuzu

Bu rehber, .NET Conference 2026 sunumu için hazırlanmış Copilot Agent'larının nasıl kullanılacağını gösterir.

## 📋 İçindekiler
1. [Agent'lara Hızlı Bakış](#agentlara-hızlı-bakış)
2. [Kullanım Yöntemleri](#kullanım-yöntemleri)
3. [Örnek Senaryolar](#örnek-senaryolar)
4. [Demo Akışı](#demo-akışı)

## Agent'lara Hızlı Bakış

| Agent | Rol | Kullanım Alanı |
|-------|-----|----------------|
| 🔧 **Backend Developer** | Senior .NET Developer | API endpoints, domain models, EF migrations |
| 🎨 **Frontend Developer** | Vue.js UI/UX Expert | Vue components, composables, TypeScript |
| 📊 **Business Analyst** | Requirements Expert | User stories, acceptance criteria, iş kuralları |
| 🚀 **DevOps Engineer** | CI/CD Specialist | Pipeline'lar, Docker, Kubernetes |
| 🧪 **QA Engineer** | Testing Expert | Unit tests, integration tests, E2E tests |

## Kullanım Yöntemleri

### Yöntem 1: VS Code Copilot Chat (Önerilen)

1. **Ctrl+Shift+I** (veya Cmd+Shift+I) ile Copilot Chat'i aç
2. Agent dosyasını context'e ekle
3. Görevi tanımla

```
@workspace Şu agent persona'sını kullan:
#file:.github/agents/backend-developer.md

US-001 için AddVehicleCommand ve Handler oluştur.
Context: #file:docs/business/US-001-add-vehicle-to-inventory.md
         #file:docs/domain-model/entity-vehicle.md
```

### Yöntem 2: Inline Chat

1. Kod editöründe **Ctrl+I** (veya Cmd+I)
2. Agent içeriğini kopyala-yapıştır
3. Spesifik görev ver

```
[Backend Developer agent içeriğini buraya yapıştır]

Görev: Vehicle entity için Repository implementasyonu yaz.
```

### Yöntem 3: Custom Instructions (Kalıcı)

1. **Settings** > **GitHub Copilot** > **Edit Instructions**
2. Sık kullandığın agent'ın içeriğini yapıştır
3. Artık her promptta o agent rolünde çalışır

## Örnek Senaryolar

### 📝 Senaryo 1: Yeni User Story Yazma

**Agent**: Business Analyst  
**Girdi**: Domain model ve mockup  
**Çıktı**: Detaylı user story

```markdown
@workspace #file:.github/agents/business-analyst.md

Araç fiyat güncelleme özelliği için user story yaz.

Domain: #file:docs/domain-model/entity-vehicle.md
Statik Data: #file:docs/static-data/enums.md

Kullanıcı: Bayi yöneticisi
Amaç: Piyasa koşullarına göre araç fiyatlarını güncelleyebilmek
İş Kuralları:
- Fiyat güncellemesi log'lanmalı
- Satış fiyatı alış fiyatından düşük olamaz
- Minimum kar marjı %5 olmalı
```

### 🔧 Senaryo 2: Backend API Endpoint

**Agent**: Backend Developer  
**Girdi**: User story  
**Çıktı**: Command, Handler, Validator, Controller

```markdown
@workspace #file:.github/agents/backend-developer.md

US-001 için uçtan uca API endpoint geliştir.

Context: 
#file:docs/business/US-001-add-vehicle-to-inventory.md
#file:docs/domain-model/entity-vehicle.md
#file:docs/domain-model/value-object-vin.md
#file:docs/domain-model/value-object-money.md
#file:docs/prompts/01-create-api-endpoint.md

Gereksinimler:
1. AddVehicleCommand ve Handler
2. FluentValidation validator
3. VehiclesController POST endpoint
4. Unit tests (xUnit, Moq, FluentAssertions)

Kodlama standartları:
#file:docs/architectural-overview/02-coding-standards.md
```

### 🎨 Senaryo 3: Vue Component Geliştirme

**Agent**: Frontend Developer  
**Girdi**: UI mockup ve user story  
**Çıktı**: Vue component, composable, TypeScript types

```markdown
@workspace #file:.github/agents/frontend-developer.md

Araç ekleme formu component'ini oluştur.

UI Mockup: #file:docs/ui/vehicle-add-form.html
User Story: #file:docs/business/US-001-add-vehicle-to-inventory.md
Static Data: #file:docs/static-data/enums.md

İstenenler:
1. VehicleForm.vue component (Composition API)
2. useVehicleForm composable
3. TypeScript interfaces
4. Form validation
5. Loading ve error states

Standartlar:
#file:docs/architectural-overview/02-coding-standards.md
```

### 🧪 Senaryo 4: Test Senaryoları

**Agent**: QA Engineer  
**Girdi**: User story ve domain model  
**Çıktı**: Unit tests, integration tests

```markdown
@workspace #file:.github/agents/qa-engineer.md

US-001 için kapsamlı test suite'i oluştur.

Context:
#file:docs/business/US-001-add-vehicle-to-inventory.md
#file:docs/domain-model/entity-vehicle.md

Test türleri:
1. Unit tests - AddVehicleCommandHandler
2. Integration tests - VehicleRepository
3. API integration tests - POST /api/v1/vehicles

Senaryolar:
- Happy path
- Duplicate VIN hatası
- Validation hataları
- Edge cases (geçersiz yıl, negatif fiyat, vb.)
```

### 🚀 Senaryo 5: CI/CD Pipeline

**Agent**: DevOps Engineer  
**Girdi**: Proje yapısı  
**Çıktı**: GitHub Actions workflow, Dockerfile

```markdown
@workspace #file:.github/agents/devops-engineer.md

Backend ve frontend için CI/CD pipeline'ları oluştur.

Teknoloji Stack:
#file:docs/architectural-overview/01-technology-stack.md

Gereksinimler:
1. GitHub Actions workflow (build, test, deploy)
2. Backend Dockerfile (.NET 8)
3. Frontend Dockerfile (Nuxt 3)
4. Docker Compose (local development)
5. SonarQube entegrasyonu
6. Container registry push

Ortamlar: Development, Staging, Production
```

## Demo Akışı (Sunum İçin)

### 🎬 Demo 1: Sıfırdan Feature Geliştirme (15 dk)

#### Adım 1: Business Analyst - User Story (2 dk)
```markdown
@workspace #file:.github/agents/business-analyst.md

"Araç durumu güncelleme" özelliği için user story yaz.
Roller: Satış danışmanı, Bayi yöneticisi
Context: #file:docs/domain-model/entity-vehicle.md
         #file:docs/static-data/enums.md (VehicleStatus)
```

**Beklenen Çıktı**: US-003 dokümantasyonu

#### Adım 2: Backend Developer - API Endpoint (5 dk)
```markdown
@workspace #file:.github/agents/backend-developer.md

Yeni oluşturulan US-003 için API geliştir.
Context: [Adım 1'de üretilen user story]
         #file:docs/domain-model/entity-vehicle.md
```

**Beklenen Çıktı**:
- `UpdateVehicleStatusCommand.cs`
- `UpdateVehicleStatusCommandHandler.cs`
- `UpdateVehicleStatusCommandValidator.cs`
- `VehiclesController.cs` (PATCH endpoint)

#### Adım 3: Frontend Developer - UI Component (5 dk)
```markdown
@workspace #file:.github/agents/frontend-developer.md

Araç durum güncelleme için dropdown component'i ekle.
Liste sayfasına status badge'i ve değiştirme butonu ekle.
Context: [US-003]
         #file:docs/ui/vehicle-list-page.html
         #file:docs/static-data/enums.md
```

**Beklenen Çıktı**:
- `VehicleStatusBadge.vue`
- `VehicleStatusSelector.vue`
- `useVehicleStatus.ts` composable

#### Adım 4: QA Engineer - Test Yazma (2 dk)
```markdown
@workspace #file:.github/agents/qa-engineer.md

US-003 için unit ve integration testleri yaz.
Context: [Backend code from Adım 2]
```

**Beklenen Çıktı**:
- `UpdateVehicleStatusCommandHandlerTests.cs`
- Test senaryoları (happy path, error cases)

#### Adım 5: Sonuç Gösterimi (1 dk)
- ✅ User story dokümantasyonu
- ✅ Backend API endpoint'i
- ✅ Frontend component'leri
- ✅ Test coverage
- ⏱️ **Toplam süre**: ~15 dakika (manuel olarak günler sürerdi)

### 🎬 Demo 2: Multi-Agent İş Birliği (10 dk)

Aynı feature için farklı agent'ları sırayla kullanarak end-to-end geliştirme göster:

```
Business Analyst → Backend Developer → Frontend Developer → QA Engineer → DevOps Engineer
```

Her adımda önceki agent'ın çıktısı bir sonraki agent'a context olarak veriliyor.

## 💡 İpuçları ve Best Practices

### ✅ Yapılması Gerekenler

1. **Context Zengin Olsun**
   ```markdown
   # İyi ✅
   Context: #file:docs/business/US-001.md
            #file:docs/domain-model/entity-vehicle.md
            #file:docs/architectural-overview/02-coding-standards.md
   
   # Kötü ❌
   Araç ekleme feature'ı için kod yaz.
   ```

2. **Spesifik Görevler Ver**
   ```markdown
   # İyi ✅
   AddVehicleCommand, Handler ve Validator oluştur.
   xUnit, Moq ve FluentAssertions kullan.
   
   # Kötü ❌
   Backend kodu yaz.
   ```

3. **Standartları Belirt**
   ```markdown
   Kodlama standartları: #file:docs/architectural-overview/02-coding-standards.md
   Proje yapısı: #file:docs/architectural-overview/03-project-structure.md
   ```

### ❌ Yapılmaması Gerekenler

1. Context olmadan karmaşık görev verme
2. Birden fazla agent'ı aynı anda karıştırma
3. Agent çıktısını review etmeden kabul etme
4. Standartları ve kuralları belirtmeden kod ürettirme

## 🔄 Iteratif Geliştirme

Agent ilk seferde tam istediğiniz çıktıyı vermezse:

```markdown
# İlk prompt
@backend-dev AddVehicleCommand oluştur

# Revizyon prompt'u
Güzel ama şunları da ekle:
- Nullable reference types kullan
- ILogger ekle
- CancellationToken parametresi ekle
- FluentValidation validator yaz
```

## 📊 Karşılaştırma: Manuel vs Agent

| Görev | Manuel Süre | Agent ile | Kazanç |
|-------|-------------|-----------|--------|
| User Story Yazma | 2 saat | 5 dk | %96 |
| API Endpoint (CRUD) | 4 saat | 10 dk | %96 |
| Vue Component | 3 saat | 15 dk | %92 |
| Unit Tests | 2 saat | 5 dk | %96 |
| CI/CD Pipeline | 4 saat | 20 dk | %92 |
| **TOPLAM** | **15 saat** | **55 dk** | **%94** |

> ⚠️ **Not**: Bu rakamlar demo amaçlıdır. Gerçek projede review ve refinement süresi de eklenmeli.

## 🎯 Sunum Talking Points

1. **"Spec-Oriented Development"**
   - Dokümantasyon önce, kod sonra
   - AI'a doğru context ver, doğru çıktı al

2. **"Agent Specialization"**
   - Her agent kendi alanında expert
   - Multi-agent collaboration > single agent

3. **"Human in the Loop"**
   - AI kod üretir, insan review eder
   - %100 otomasyon değil, %400 hızlanma

4. **"Learning Curve"**
   - Prompt engineering becerisi gerekir
   - Agent'ları iteratif olarak iyileştir

## 📚 Ek Kaynaklar

- [Prompt Engineering Guide](https://www.promptingguide.ai/)
- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)

---

**Hazırlayan**: Burak  
**Etkinlik**: .NET Conference 2026  
**Tarih**: Şubat 2026
