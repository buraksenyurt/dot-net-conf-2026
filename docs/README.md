# Doküman Yapısı - Spec-Oriented Development

Bu klasör, yapay zeka asistanları ile etkileşimde kullanılan spesifikasyon dokümanlarını içerir.

## Klasör Yapısı

### 📋 architectural-overview/
Sistemin genel mimari yapısı, kullanılan teknolojiler, kodlama standartları ve teknik kılavuzlar.

### 📊 business/
Feature bazlı kullanıcı hikayeleri (user stories) ve iş gereksinimleri.

### 🏗️ domain-model/
DDD (Domain-Driven Design) yaklaşımıyla modellenmiş domain nesneleri:
- Entity'ler
- Value Object'ler
- Aggregate Root'lar
- Domain Event'ler

### 🎨 ui/
Mock-up ekranların HTML formatındaki tasarımları ve UI spesifikasyonları.

### 📦 static-data/
Sabit veriler, parametreler, lookup değerleri ve referans datalar.

### 🤖 prompts/
Yapay zeka asistanlarına kullandırılan prompt şablonları:
- API endpoint oluşturma
- EF Migration hazırlama
- Vue component geliştirme
- Test senaryoları oluşturma

## Kullanım

Bu dokümanlar, GitHub Copilot ve diğer AI asistanlarına context olarak sunularak:
1. Tutarlı kod üretimi sağlar
2. Domain kurallarına uygun geliştirme yapar
3. Standartlara uygun mimari oluşturur
4. Otomatik test senaryoları üretir

## Simülasyon Senaryosu

Bu demo için **Araç Envanter Yönetimi** modülü üzerinden basitleştirilmiş bir senaryo hazırlanmıştır.
