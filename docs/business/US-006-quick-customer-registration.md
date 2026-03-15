# US-006: Araç Opsiyonlama Sırasında Hızlı Müşteri Kaydı

## Kullanıcı Hikayesi
**Rol olarak**: Satış danışmanı  
**İstiyorum ki**: Araç opsiyonlama formunu terk etmeden yeni bir müşteri kaydı oluşturabileyim  
**Böylece**: Sistemde henüz kaydı bulunmayan bir müşteri için hızlıca opsiyon oluşturabileyim ve iş akışım kesintisiz devam etsin

## Kabul Kriterleri

### AC-1: Modal Açma
- [ ] Opsiyonlama formundaki müşteri arama alanı yanında "Yeni Müşteri" butonu (`btn-outline-success`, `bi-person-plus` ikon) bulunmalı
- [ ] Butona tıklandığında sayfa yenilenmeden Bootstrap 5 modal açılmalı

### AC-2: Müşteri Formu
- [ ] Modal; Ad, Soyad, E-posta, Telefon alanlarını içermeli; tüm alanlar zorunlu olmalı
- [ ] Müşteri Tipi seçimi (Bireysel / Kurumsal) radio buton olarak sunulmalı; varsayılan Bireysel olmalı
- [ ] Kurumsal seçildiğinde Firma Adı ve Vergi No alanları koşullu olarak görünmeli ve zorunlu hale gelmeli
- [ ] E-posta adresi RFC 5322 formatında doğrulanmalı

### AC-3: Kayıt ve Otomatik Seçim
- [ ] "Müşteriyi Ekle" butonuna tıklandığında `POST /v1/customers` endpoint'i çağrılmalı
- [ ] Başarılı kayıt (201 Created) sonrası modal kapanmalı
- [ ] Yeni oluşturulan müşteri, sayfayı terk etmeden opsiyonlama formuna otomatik seçilmeli

### AC-4: Hata Yönetimi
- [ ] 409 Conflict (e-posta zaten kayıtlı) hatası modal içinde inline alert olarak gösterilmeli
- [ ] 400 Bad Request (doğrulama hatası) durumunda hata mesajı modal içinde gösterilmeli
- [ ] Hata durumunda modal kapanmamalı; kullanıcı formu düzeltebilmeli

## İş Kuralları

### BR-1: Aynı İş Kuralları
Hızlı müşteri kayıt formu, US-004'te tanımlanan müşteri oluşturma iş kurallarının tamamını uygular:
- E-posta adresi sistem genelinde benzersiz olmalıdır (409 Conflict).
- Bireysel müşteri için ad, soyad, e-posta ve telefon zorunludur.
- Kurumsal müşteri için ek olarak firma adı ve vergi numarası zorunludur.

### BR-2: Otomatik Seçim
Yeni kayıt başarıyla oluşturulduğunda müşteri nesnesi (`{ id, firstName, lastName, email, phone, customerType, ... }`) anında opsiyonlama formuna seçilir; arama yapılmasına gerek kalmaz.

### BR-3: Modal Durumu Sıfırlama
Modal her açıldığında form alanları ve hata mesajları temizlenmeli; Müşteri Tipi varsayılan olarak Bireysel seçili gelmelidir.

## Teknik Notlar

### API Endpoint
```
POST /v1/customers
     Body:     { firstName, lastName, email, phone, customerType, companyName?, taxNumber? }
     Response: 201 Created { id: guid } | 400 Bad Request | 409 Conflict
```

### Tip Tanımları (Frontend)
```typescript
interface CreateCustomerRequest {
  firstName: string
  lastName: string
  email: string
  phone: string
  customerType: 'Individual' | 'Corporate'
  companyName?: string
  taxNumber?: string
}
```

### Bağımlılıklar
- `api/customer.ts`: `createCustomer(request: CreateCustomerRequest): Promise<{ id: string }>` metodu
- Bootstrap 5 `Modal` sınıfı (`@types/bootstrap` devDependency)
- `VehicleOptionForm.vue` — müşteri seçim state'i (`selectedCustomer`, `selectCustomer()`)
- US-004: Müşteri Yönetimi (aynı endpoint ve iş kuralları)

## Test Senaryoları

### Senaryo 1: Bireysel Müşteri Hızlı Kaydı
1. Opsiyonlama formunda "Yeni Müşteri" butonuna tıkla
2. Modal açılır; Ad, Soyad, E-posta, Telefon doldurulur; Bireysel seçili
3. "Müşteriyi Ekle" butonuna tıkla
4. 201 döner; modal kapanır; yeni müşteri forma otomatik seçilir

### Senaryo 2: Kurumsal Müşteri Hızlı Kaydı
1. Modal açılır; Kurumsal seçilir → Firma Adı ve Vergi No alanları görünür
2. Tüm alanlar doldurulur
3. "Müşteriyi Ekle" butonuna tıkla
4. 201 döner; modal kapanır; yeni kurumsal müşteri forma otomatik seçilir

### Senaryo 3: Duplicate E-posta (409 Conflict)
1. Sistemde kayıtlı bir e-posta ile kayıt dene
2. 409 Conflict döner
3. Modal kapanmaz; "Bu e-posta adresi zaten kayıtlı" hata mesajı modal içinde gösterilir

### Senaryo 4: Kurumsal — Firma Adı Boş
1. Müşteri Tipi Kurumsal seçilir; Firma Adı boş bırakılır
2. "Müşteriyi Ekle" butonuna tıkla
3. 400 Bad Request döner; hata mesajı modal içinde gösterilir

### Senaryo 5: Modal Vazgeç
1. Modal açılır, alanlar doldurulur
2. "Vazgeç" butonuna veya × ikonuna tıklanır
3. Modal kapanır; opsiyonlama formunda herhangi bir değişiklik olmaz

## UI/UX Notları
- Modal başlığı: **"Yeni Müşteri Ekle"** (`bi-person-plus` ikon, `text-success`)
- Kaydetme butonu: `btn-success`, `bi-person-check` ikon — "Müşteriyi Ekle"
- Vazgeç butonu: `btn-outline-secondary`, `bi-x-circle` ikon
- Hata alert'i: `alert-danger`, `bi-exclamation-triangle-fill` ikon — modal üstünde inline
- Kurumsal alanlar `v-if="customerType === 'Corporate'"` ile koşullu gösterilir

## Ekran Mockup Referansı
`docs/ui/vehicle-option-form.html` — Modal bölümü

## Öncelik & Planlama

| Alan | Değer |
|---|---|
| Öncelik | Orta |
| Story Points | 2 |
| Sprint | Sprint 2 |
| Bağımlı Hikayeler | US-003 (Araç Opsiyonlama), US-004 (Müşteri Yönetimi) |
