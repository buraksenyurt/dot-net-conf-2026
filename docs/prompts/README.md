# Prompt Template Index

Bu klasör, GitHub Copilot ve diğer AI asistanlarına kullandırılacak prompt şablonlarını içerir.

## Mevcut Prompt'lar

### Backend Development
1. **01-create-api-endpoint.md** - Uçtan uca API endpoint geliştirme (Command, Query, Handler, Controller, Tests)
2. **02-create-ef-migration.md** - Entity Framework migration oluşturma ve configuration

### Frontend Development
3. **03-create-vue-component.md** - Vue 3 Composition API ile component geliştirme

### Test Development
4. **04-create-unit-tests.md** - Unit test oluşturma (xUnit, Moq, FluentAssertions)
5. **05-create-integration-tests.md** - Integration test oluşturma (TestContainers)

### Documentation
6. **06-generate-api-documentation.md** - OpenAPI/Swagger dokümentasyonu
7. **07-create-user-story.md** - User story dokümanı oluşturma

## Prompt Kullanım Rehberi

### 1. Context Hazırlama
Prompt kullanmadan önce gerekli context dosyalarını aç:
- User story (`docs/business/`)
- Domain model (`docs/domain-model/`)
- Architectural overview (`docs/architectural-overview/`)
- UI mockup (`docs/ui/`)

### 2. Prompt Özelleştirme
Template'teki placeholder'ları doldur:
- `[FeatureName]` - İlgili feature adı
- `[US-XXX]` - User story numarası
- `[Domain entities]` - İlgili domain modeller

### 3. Copilot'a Gönderme

#### VS Code Chat Panel
```
@workspace [prompt içeriği]
Context: #file:docs/business/US-001-xxx.md
         #file:docs/domain-model/entity-xxx.md
```

#### Inline Chat (Ctrl+I / Cmd+I)
```
[Prompt içeriğini yapıştır]
```

#### Custom Agent Kullanımı
```
@agent-backend-developer [kısa açıklama]
Context: [ilgili dosyalar]
Prompt: #file:docs/prompts/01-create-api-endpoint.md
```

### 4. Çıktıyı Review Et
- Kodlama standartlarına uygunluk kontrol et
- Testleri çalıştır
- SonarQube analizi yap
- Manual code review

## Yeni Prompt Ekleme

### Prompt Template Yapısı
```markdown
# Prompt: [Kısa Başlık]

## Amaç
[Bu prompt'un ne için kullanılacağı]

## Prompt Template
```markdown
[Asıl prompt içeriği - AI'ya gönderilecek text]
```

## Kullanım
[Nasıl kullanılacağına dair örnekler]

## Beklenen Çıktı Örneği
[AI'dan beklenen çıktı örnekleri]

## Notlar
[Önemli notlar ve uyarılar]
```

### Prompt Best Practices

1. **Açık ve Spesifik Ol**
   - Belirsiz ifadeler kullanma
   - Beklentileri net belirt
   - Örnek ver

2. **Context Sağla**
   - İlgili dosyaları referans et
   - Domain bilgisini dahil et
   - Kodlama standartlarını belirt

3. **Kısıtlamaları Belirt**
   - Kullanılacak teknolojiler
   - Uyulması gereken pattern'ler
   - Yasaklı pratikler

4. **Doğrulama Kriterleri Ekle**
   - Ne zaman başarılı sayılacak
   - Hangi testler geçmeli
   - Hangi metrikler sağlanmalı

5. **Örnek Çıktı Ver**
   - Beklenen kod yapısını göster
   - Naming convention'ları örneklendir
   - Doğru ve yanlış örnekler ver

## İteratif Geliştirme

Prompt'lar sürekli geliştirilmelidir:
1. İlk versiyonu oluştur
2. Test et ve çıktıyı değerlendir
3. Eksiklikleri tespit et
4. Prompt'u iyileştir
5. Tekrarla

Her başarılı/başarısız kullanımdan sonra prompt'u güncelle.

## Prompt Versiyonlama

Büyük değişikliklerde versiyon no ekle:
- `01-create-api-endpoint-v1.md`
- `01-create-api-endpoint-v2.md`

Değişiklik notlarını dokümante et.

## AI Model Uyumluluğu

Farklı AI modeller farklı prompt stillerine daha iyi yanıt verebilir:

### Claude Sonnet
- Detaylı, yapılandırılmış prompt'ları sever
- Context'e iyi uyum sağlar
- Kod kalitesi yüksek

### GPT-4
- Örneklerden iyi öğrenir
- Yaratıcı çözümler üretebilir
- Bazen over-engineer edebilir

### Gemini
- Teknik dokümantasyonu iyi anlar
- Hızlı iterasyon için uygun
- Validation konusunda dikkatli olmalı

## Faydalı Kaynaklar

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [Prompt Engineering Guide](https://www.promptingguide.ai/)
- [Best Practices for AI-Assisted Coding](https://github.blog/2023-06-20-how-to-write-better-prompts-for-github-copilot/)
