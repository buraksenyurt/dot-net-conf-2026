---
name: create-vue-component
description: Vue 3 Composition API ile bileşen geliştirme. docs/ui/ altındaki HTML wireframe kaynak kabul edilerek page component, organism component, composable ve TypeScript type tanımları oluşturulur. Bootstrap 5, form validation, API entegrasyonu ve erişilebilirlik standartlarına uyar.
---

# Skill: Vue 3 Component Geliştirme

## Ne Zaman Kullanılır
- Yeni bir sayfaya ait Vue component'leri oluşturulacaksa
- HTML wireframe'den Vue implementasyonuna geçilecekse
- Form, liste veya dashboard component'i yazılacaksa
- Composable ile reusable logic ayrıştırılacaksa

## Gerekli Context Dosyaları
- `docs/ui/[feature]-*.html` — HTML Wireframe (UI'ın kaynak gerçeği)
- `docs/business/US-XXX-*.md` — User Story ve Acceptance Criteria
- `docs/architectural-overview/02-coding-standards.md` — Kodlama standartları

## Proje Yapısı

```
src/frontend/src/
├── pages/
│   └── [feature]/
│       └── [PageName].vue           # Route'a bağlı sayfa bileşeni
├── components/
│   └── organisms/
│       └── [ComponentName].vue      # Kompleks, birden fazla molecule içeren
├── composables/
│   └── use[FeatureName].ts          # Sayfa/form logic'i
├── services/
│   └── [feature]Api.ts              # Axios API çağrıları
└── types/
    └── [feature].ts                 # TypeScript interface'ler ve enum'lar
```

## Adım Adım Süreç

### 1. TypeScript Types Tanımla

```typescript
// types/[feature].ts
export interface [EntityName] {
  id: string
  // properties...
}

export interface Create[EntityName]Request {
  // request fields...
}

export enum [EnumName] {
  Value1 = 'Value1',
  Value2 = 'Value2'
}
```

### 2. API Service Oluştur

```typescript
// services/[feature]Api.ts
import axios from 'axios'
import type { [EntityName], Create[EntityName]Request } from '@/types/[feature]'

const BASE_URL = '/api/v1/[entities]'

export const [feature]Api = {
  async getAll(): Promise<[EntityName][]> {
    const { data } = await axios.get<[EntityName][]>(BASE_URL)
    return data
  },

  async create(request: Create[EntityName]Request): Promise<string> {
    const { data } = await axios.post<string>(BASE_URL, request)
    return data
  }
}
```

### 3. Composable Yaz

```typescript
// composables/use[FeatureName].ts
import { ref, reactive } from 'vue'
import { useRouter } from 'vue-router'
import type { Create[EntityName]Request } from '@/types/[feature]'
import { [feature]Api } from '@/services/[feature]Api'

export function use[FeatureName]() {
  const router = useRouter()
  const loading = ref(false)
  const error = ref<string | null>(null)

  const form = reactive<Create[EntityName]Request>({
    // initial values
  })

  const submit = async () => {
    loading.value = true
    error.value = null
    try {
      await [feature]Api.create(form)
      await router.push('/[entities]')
    } catch (e) {
      error.value = 'İşlem sırasında bir hata oluştu.'
    } finally {
      loading.value = false
    }
  }

  return { form, loading, error, submit }
}
```

### 4. Organism Component Oluştur

```vue
<!-- components/organisms/[ComponentName].vue -->
<script setup lang="ts">
import { use[FeatureName] } from '@/composables/use[FeatureName]'

const { form, loading, error, submit } = use[FeatureName]()
</script>

<template>
  <form @submit.prevent="submit" novalidate>
    <!-- Bootstrap 5 form layout -->
    <div class="mb-3">
      <label for="field" class="form-label">Alan Adı <span class="text-danger">*</span></label>
      <input
        id="field"
        v-model="form.field"
        type="text"
        class="form-control"
        :class="{ 'is-invalid': validationError }"
        required
        aria-describedby="field-error"
      />
      <div id="field-error" class="invalid-feedback">Lütfen bu alanı doldurun.</div>
    </div>

    <div v-if="error" class="alert alert-danger" role="alert">{{ error }}</div>

    <button type="submit" class="btn btn-primary" :disabled="loading">
      <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
      Kaydet
    </button>
  </form>
</template>
```

### 5. Page Component Oluştur

```vue
<!-- pages/[feature]/[PageName].vue -->
<script setup lang="ts">
import [ComponentName] from '@/components/organisms/[ComponentName].vue'
</script>

<template>
  <div class="container-fluid">
    <div class="row mb-4">
      <div class="col">
        <h1 class="h3">Sayfa Başlığı</h1>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="/">Ana Sayfa</a></li>
            <li class="breadcrumb-item active">Sayfa Adı</li>
          </ol>
        </nav>
      </div>
    </div>
    <[ComponentName] />
  </div>
</template>
```

## Kodlama Kuralları
- `<script setup lang="ts">` syntax kullan (Options API kullanma)
- `defineProps<Interface>()` ile type-safe props
- `defineEmits<{ eventName: [ArgType] }>()` ile typed emits
- Bootstrap 5 class'ları HTML wireframe'den al
- Tüm form elementlerinde `id` ve `aria-*` attribute'ları ekle
- Loading durumunda butonları `disabled` yap
- API hatalarını kullanıcıya anlaşılır mesajla göster
- `v-model` ile two-way binding, `computed` ile derived state

## Kontrol Listesi
- [ ] HTML wireframe incelendi
- [ ] TypeScript interface'ler tanımlandı
- [ ] Composable ile form logic ayrıştırıldı
- [ ] Bootstrap 5 form validation sınıfları eklendi
- [ ] ARIA etiketleri mevcut (accessibility)
- [ ] Loading state yönetildi
- [ ] Hata durumları gösteriliyor
- [ ] `yarn dev` ile test edildi
