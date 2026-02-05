# GitHub Copilot Agent'ları

Bu klasör, projede kullanılan özelleştirilmiş AI agent persona'larını içerir. Her agent belirli bir rol ve sorumluluk için optimize edilmiştir.

## Mevcut Agent'lar

### 1. **Senior Software Developer** (Backend)
**Dosya**: `backend-developer.md`  
**Uzmanlık**: .NET, C#, Clean Architecture, CQRS, Domain-Driven Design  
**Kullanım**: Backend API endpoint'leri, domain modeller, business logic

### 2. **UI/UX Expert** (Frontend)
**Dosya**: `frontend-developer.md`  
**Uzmanlık**: Vue.js, TypeScript, Responsive Design, Accessibility  
**Kullanım**: Vue component'leri, composables, user interface

### 3. **Senior Business Analyst**
**Dosya**: `business-analyst.md`  
**Uzmanlık**: User story yazımı, requirement analizi, acceptance criteria  
**Kullanım**: Business dokümanları, user story'ler, süreç analizi

### 4. **DevOps Engineer**
**Dosya**: `devops-engineer.md`  
**Uzmanlık**: CI/CD, Docker, Kubernetes, GitHub Actions  
**Kullanım**: Pipeline'lar, deployment, infrastructure

### 5. **QA Engineer**
**Dosya**: `qa-engineer.md`  
**Uzmanlık**: Unit testing, Integration testing, Test automation  
**Kullanım**: Test senaryoları, test automation, quality assurance

## Agent Kullanım Yöntemleri

### Yöntem 1: VS Code ile Copilot Chat
```
@workspace #file:.github/agents/backend-developer.md

[Agent rolüne uygun görev tanımı]
Context: #file:docs/business/US-001-xxx.md
```

### Yöntem 2: Inline Chat (Ctrl+I / Cmd+I)
```
Aşağıdaki agent persona'sını kullan:
[Agent dosyasının içeriğini kopyala]

Görev: [Spesifik istek]
```

### Yöntem 3: Custom Instructions
Agent içeriğini GitHub Copilot Settings > Instructions kısmına ekleyerek kalıcı hale getir.

### Yöntem 4: Komut Bazlı (Alias)
```
# Örnek kullanım
@backend-dev US-001 için AddVehicleCommand ve Handler oluştur
@frontend-dev Vehicle ekleme formu component'ini oluştur
@qa-engineer US-001 için test senaryoları yaz
```

## Agent Seçim Rehberi

| Görev Tipi | Kullanılacak Agent | Context Dosyaları |
|------------|-------------------|-------------------|
| Backend API endpoint | Senior Software Developer | business/, domain-model/, prompts/01-* |
| Database migration | Senior Software Developer | domain-model/, architectural-overview/ |
| Vue component | UI/UX Expert | ui/, business/, prompts/03-* |
| User story yazma | Senior Business Analyst | domain-model/, static-data/ |
| Test senaryoları | QA Engineer | business/, domain-model/ |
| CI/CD pipeline | DevOps Engineer | architectural-overview/ |

## Best Practices

### ✅ Yapılması Gerekenler
- Her görev için doğru agent'ı seç
- İlgili context dosyalarını ekle
- Spesifik ve ölçülebilir görevler tanımla
- Agent çıktısını her zaman review et

### ❌ Yapılmaması Gerekenler
- Tek bir agent'a tüm görevleri yükleme
- Context dosyası olmadan karmaşık görev verme
- Agent çıktısını blind trust ile kabul etme
- Kodlama standartlarını atlamaya izin verme
