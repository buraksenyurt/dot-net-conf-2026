# Yapay Zeka, Yılların Koduna Karşı: AI Tabanlı Legacy Modernizasyonu

Legacy bir sistemi modernize etmek için yapay zeka teknolojilerinden nasıl yararlanıyoruz, ne gibi zorluklarla karşılaşıyoruz ve vardığımız sonuçlar...

## Legacy System

Milenyum başında geliştirilmeye başlamış olan bayi yönetimi sistemi(Dealer Management System - DMS), tamamen Microsoft .net teknolojileri üzerine kurgulanmıştır. Bu nedenle .net Framework'ün zaman içerisindeki değişimine bağlı olarak yer yer modernize edilmiş ve güncellenmiştir. Şu anda .Net Framework 4.8 sürümünü kullanmaktadır. Sistem, bayi operasyonlarını yönetmek için kritik öneme sahip birçok modül içermektedir. Tabii Microsoft'un .Net Framework için olan desteği 2029 yılında sona erecektir. Bu nedenle, sistemin gelecekteki sürdürülebilirliği için modernizasyon kaçınılmazdır.

Genel olarak N-Tier mimarisine göre düzenlenmiş bir sistemdir. Presentation, Business Logic Layer ve Data Acces Layer olmak üzere üç ana katmandan oluşan bir mimari üzerine kurgulanmıştır. Daha önceden var olan Façade katmanı ilk modernizasyon çalışması kapsamında kaldırılmıştır. Sistem Microsoft SQL Server veritabanı kullanmaktadır. İş kuralları ve süreçleri modül bazında son derece karmaşık ve iç içe geçmiş olabilir. Bu noktada SQL Server'ın stored procedure avantajları gözetilerek iş kuralları ve süreçlerin bir kısmı veritabanı katmanında da uygulanmıştır. Dolayısıyla kod ve veritabanına yayılmış iş kuralları ve süreçleri mecvuttur.

Onlarca yıllık bir uygulama söz konusu olduğundan beş milyon satırdan fazla bir kod tabanı söz konusudur. Binlerce ekran, yüzlerce stored procedure, tera baytlarca veri, onlarca servis beş ana modül etrafında birleşir. Bu modüller finansal hizmetler, araç, satış sonrası hizmetler, yedek parça, müşteri olarak sıralanabilir. Sistem, yüzlerce bayi tarafından kullanılmakta olup, milyonlarca müşteriye hizmet vermektedir.

Sistemle entegre çalışan birçok uygulama da vardır. Örneğin ayrı bir raporlama sistemi bulunmaktadır. Bu sistem datawarehouse mimarisi üzerine kurulmuş olup, ETL süreçleriyle ana sistemden verileri çekmektedir. Raporların hazırlanması için planlanmış işler kullanılır. 200den fazla Job vardır ve bunların bazılarının çalışma süresi saatler mertebesindedir. Job'ların çoğu doğrudan Stored Procedure işletmekle kimisi de Microsoft'un SSIS *(Sql Server Integration Services)* hizmetleri şeklinde çalıştırılmaktadır. Bazı raporlar anlık üretilebilen türdedir ve bunlar için Liquid rapor şablonları kullanılmaktadır. Daha önceki dönemlerde Microsoft'un SSRS *(Microsoft SQL Server Reporting Services)* raporları da kullanılmıştır.

Sistem aynı zaman regülasyonlar içeren dış servislere de bağımlılıklar içerir. Örneğin, elektronik faturlama ve irsaliye sistemleri, POS tabanlı ödeme cihazları, kurum için yazılmış yeni nesil uygulamalar vb. Bu sistemlerde haberleşme için ağırlıklı olarak SOAP ve REST tabanlı web servisleri kullanılmaktadır. Ana sistemden dışarıya açılan fonksiyonellikler içinse  XML Web Servisler ve WCF servisleri kullanılmaktadır. Ayrıca yeni nesil uygulamaların ihtiyaç duyduğu veya karşılıklı olarak dahil olunması gereken süreçlerde asenkron mesajlaşma altyapısı kullanılmaktadır. Bunun için Rabbit MQ tercih edilmiştir. Modüller de kendi aralarında kullandığı ortak süreçlere sahiptir. Ortak ve tek bir veritabanı sistemi olduğundan modüller arası veri paylaşımı doğrudan veritabanı katmanından ve iş nesneleri üzerinden yapılmaktadır.

Uygulamanın dağıtımı ilk zamanlarda kurum içi geliştirilmiş bir uygulama tarafından zaman bazlı planlamalara bağlı kalınarak yapılmaktaydı. Son yıllarda yapılan modernizasyon çalışmaları kapsamında DevOps prensiplerine uygun olarak Azure DevOps üzerinden yürütülmektedir. Git tabanlı repolar kullanılmakta ve CI/CD süreçleri Azure DevOps Pipelines ile yönetilmektedir. Branch stratejisi olarak Git Flow tercih edilmiştir. Buna göre feature bazlı geliştirmeler yapılmakta, sprint bazlı release'ler oluşturulmakta ve ana branch'lere merge edilmektedir.

### Metriklerle Legacy Sistemimiz

Aşağıdaki tabloda sistemimizin bazı metriklerini özetlemektedir:

| Metrik                     | Değer               |
|----------------------------|---------------------|
| Kod Satırı Sayısı          | 5,000,000+          |
| Ekran Sayısı               | 1,000+              |
| Stored Procedure Sayısı    | 1,000+              |
| Veri Tabanı Boyutu         | >30 TB              |
| Entegre Uygulama Sayısı    | 50+                 |
| Kullanıcı Sayısı           | 10,000+             |
| Modül Sayısı               | 5                   |
| Rapor Sayısı               | 500+                |
| Job Sayısı                 | 200+                |

### Tekonoloji Altyapısı

| Katman                     | Teknoloji           |
|----------------------------|---------------------|
| Sunum Katmanı              | ASP.NET Web Forms   |
| İş Katmanı                 | C# (.NET Framework) |
| Veri Erişim Katmanı        | ADO.NET             |
| Veritabanı                 | Microsoft SQL Server|
| Entegrasyon                | WCF, SOAP, REST     |
| Mesajlaşma                 | Rabbit MQ           |
| Raporlama                  | SSRS, Liquid Rapor  |
| Dağıtım                    | Azure DevOps        |

### Sistemdeki Genel Problemler (2020 Öncesi)

Var olan sistem yüksek müşteri memnuniyet sağlamasına ve ihtiyaçlar tam olarak cevap vermesine rağmen gelişen teknolojiler ve artan iş gereksinimleri nedeniyle çeşitli zorluklarla karşılaşmıştır. Bu zorluklar ürünün modernize edilmesi, farklı bir mimariye geçilmesi veya parçalara ayrılarak ürünleştirilmesi noktasında engeller oluşturmaktadır. Genel hatları ile bu zorluklar şöyle özetlenebilir:

- Zamanla ön yüz formlarına karışan iş kuralları ve süreçler
- Kod tabanında biriken teknik borçlar
- Test edilebilirliğin düşük olması
- Geliştirilen müşteri taleplerine ait kurumsal hafızanın zamanla kaybolması
- Modüller arası sıkı bağımlılıklar ve zayıf soyutlamalar
- Yüksek lisanslama maliyetleri

### İlk Modernizasyon Çalışmaları (2020 - 2024)

Modernizasyon ihtiyaçlarının netleştirilmesi için 2020 öncesinde birçok fizibilite çalışması gerçekleştirilmiş ve var olan durum detaylı raporlarla açıklanmıştır. Yeni mimari modellere geçmek ve modüllerin bağımsız olarak çalıştırılabilmesi stratejik hedef olarak belirlenmiştir. Bu kapsamda ilk uzun soluklu IT4IT çalışması 2020 yılında başlatılmıştır. Bu çalışmada bir yol haritası çıkartılmış ve aşağıdaki ana adımlar atılmıştır.

- Sonarqube ile kod kalitesinin düzenli olarak ölçümlenmesi ve raporlanması.
- Teknik borçlar 1000 kişi gün maliyetinden 100 kişi gün altına düşürülmüş yer yer sıfıra indirilmiştir.
- Façade katmanı kaldırılmıştır.
- CBL katmanındaki fonksiyonellikler soyutlanmış ve ayrı bir katmana taşınmıştır.
- Tüm bileşenler için dependency injection altyapısı kurulmuştur *(Windsor Castle)*.
- Unit testler yazılmaya başlanmış ve code coverage değerlerinin kabül edilebilir seviyelere gelmesi sağlanmıştır.
- CI/CD süreçleri iyileştirilmiş ve otomasyon oranı artırılmıştır.

Bu modernizasyon çalışmaları hali hazırda devam etmektedir ancak asıl stratejik hedeflere ulaşma noktasında ürünün sıfırdan yazılma maliyetinin çok yüksek olması nedeniyle yeni yaklaşımlar araştırılmaya başlanmış ve bu kapsamda yapay zeka tabanlı modernizasyon çözümleri mercek altına alınmıştır. Yazının bundan sonraki kısımlarında son altı aylık dönem içerisinde gerçekleştirilen yapay zeka tabanlı modernizasyon çalışmaları anlatılmakta olup sonuçlar ve gelecek planları paylaşılmaktadır.

## Level 0: Birinci Aşama

### Plan

### Karşılaşılan Sorunlar

### Level 1: Kick-Off

## Sonraki Planlar ve Hedefler
