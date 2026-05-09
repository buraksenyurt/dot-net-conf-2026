# Value Object: Email

## Tanım

**Email** — Bir müşterinin e-posta adresini temsil eden, doğrulama ve normalleştirme kurallarını içinde saklayan value object.

## Özellikler

- **Value**: `string` — Normalleştirilmiş (küçük harf, trimlenmiş) e-posta adresi

## Validasyon Kuralları

| Kural | Detay |
|---|---|
| Boş olamaz | `null`, boş string veya yalnızca boşluk kabul edilmez |
| Max uzunluk | 254 karakter (RFC 5321) |
| Format | `RFC 5322`-uyumlu regex kontrolü |

## Normalleştirme

Factory method çağrıldığında:
1. Baş/son boşluklar temizlenir (`Trim`)
2. Tüm harfler küçük harfe çevrilir (`ToLowerInvariant`)

Bu sayede `User@EXAMPLE.com` ile `user@example.com` aynı e-posta olarak değerlendirilir.

## Kullanım

```csharp
var result = Email.Create("alvo.yarnsby@example.com");

if (result.IsSuccess)
{
    Console.WriteLine(result.Value!.Value); // "alvo.yarnsby@example.com"
}
else
{
    Console.WriteLine(result.Error); // Hata açıklaması
}
```

## İlgili Entity

- [Customer](entity-customer.md) — `Email` property'si bu value object'i kullanır.
