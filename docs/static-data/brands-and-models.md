# Static Data: Araç Markaları ve Modelleri

## Desteklenen Markalar ve Modeller

### Honda
```json
{
  "brandCode": "honda",
  "brandName": "Honda",
  "country": "Japan",
  "models": [
    { "modelCode": "civic", "modelName": "Civic", "segment": "C" },
    { "modelCode": "accord", "modelName": "Accord", "segment": "D" },
    { "modelCode": "crv", "modelName": "CR-V", "segment": "SUV" },
    { "modelCode": "hrv", "modelName": "HR-V", "segment": "SUV" },
    { "modelCode": "jazz", "modelName": "Jazz", "segment": "B" }
  ]
}
```

### Toyota
```json
{
  "brandCode": "toyota",
  "brandName": "Toyota",
  "country": "Japan",
  "models": [
    { "modelCode": "corolla", "modelName": "Corolla", "segment": "C" },
    { "modelCode": "camry", "modelName": "Camry", "segment": "D" },
    { "modelCode": "rav4", "modelName": "RAV4", "segment": "SUV" },
    { "modelCode": "yaris", "modelName": "Yaris", "segment": "B" },
    { "modelCode": "chr", "modelName": "C-HR", "segment": "SUV" }
  ]
}
```

### Mercedes-Benz
```json
{
  "brandCode": "mercedes",
  "brandName": "Mercedes-Benz",
  "country": "Germany",
  "models": [
    { "modelCode": "c-class", "modelName": "C-Class", "segment": "D" },
    { "modelCode": "e-class", "modelName": "E-Class", "segment": "E" },
    { "modelCode": "s-class", "modelName": "S-Class", "segment": "F" },
    { "modelCode": "glc", "modelName": "GLC", "segment": "SUV" },
    { "modelCode": "gle", "modelName": "GLE", "segment": "SUV" }
  ]
}
```

### BMW
```json
{
  "brandCode": "bmw",
  "brandName": "BMW",
  "country": "Germany",
  "models": [
    { "modelCode": "3-series", "modelName": "3 Serisi", "segment": "D" },
    { "modelCode": "5-series", "modelName": "5 Serisi", "segment": "E" },
    { "modelCode": "7-series", "modelName": "7 Serisi", "segment": "F" },
    { "modelCode": "x3", "modelName": "X3", "segment": "SUV" },
    { "modelCode": "x5", "modelName": "X5", "segment": "SUV" }
  ]
}
```

### Audi
```json
{
  "brandCode": "audi",
  "brandName": "Audi",
  "country": "Germany",
  "models": [
    { "modelCode": "a3", "modelName": "A3", "segment": "C" },
    { "modelCode": "a4", "modelName": "A4", "segment": "D" },
    { "modelCode": "a6", "modelName": "A6", "segment": "E" },
    { "modelCode": "q3", "modelName": "Q3", "segment": "SUV" },
    { "modelCode": "q5", "modelName": "Q5", "segment": "SUV" }
  ]
}
```

## Kullanım

### API Response Örneği
```json
GET /api/v1/static-data/brands

{
  "data": [
    {
      "code": "honda",
      "name": "Honda",
      "country": "Japan",
      "logo": "/assets/brands/honda.svg"
    },
    {
      "code": "toyota",
      "name": "Toyota",
      "country": "Japan",
      "logo": "/assets/brands/toyota.svg"
    }
  ]
}
```

### Frontend Constants
```typescript
// constants/brands.ts
export const BRANDS = [
  { value: 'honda', label: 'Honda' },
  { value: 'toyota', label: 'Toyota' },
  { value: 'mercedes', label: 'Mercedes-Benz' },
  { value: 'bmw', label: 'BMW' },
  { value: 'audi', label: 'Audi' }
] as const;

export type BrandCode = typeof BRANDS[number]['value'];
```

## Segment Açıklamaları

- **A Segmenti**: Mini araçlar (örn: Fiat 500)
- **B Segmenti**: Küçük araçlar (örn: Yaris, Jazz)
- **C Segmenti**: Orta segment (örn: Civic, Corolla, Golf)
- **D Segmenti**: Orta-üst segment (örn: Accord, Camry, Passat)
- **E Segmenti**: Üst segment (örn: 5 Serisi, A6, E-Class)
- **F Segmenti**: Lüks segment (örn: 7 Serisi, S-Class)
- **SUV**: Sport Utility Vehicle (örn: CR-V, RAV4, X5)

## Güncelleme Sıklığı

Bu static data yılda 1-2 kez güncellenir. Yeni model lansmanları veya marka eklemeleri durumunda revizyon yapılır.
