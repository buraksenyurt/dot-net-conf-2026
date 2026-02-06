# Yapay Zeka, Yılların Koduna Karşı: AI Tabanlı Legacy Modernizasyonu

Legacy bir sistemi modernize etmek için yapay zeka teknolojilerinden nasıl yararlanıyoruz, ne gibi zorluklarla karşılaşıyoruz ve vardığımız sonuçlar...

## Legacy System

Milenyum başında geliştirilmeye başlamış olan bayi yönetimi sistemi *(Dealer Management System - DMS)*, tamamen Microsoft .Net teknolojileri üzerine kurgulanmıştır. Bu nedenle .Net Framework'ün zaman içerisindeki değişimine bağlı olarak yer yer modernize edilmiş ve güncellenmiştir. Şu anda .Net Framework 4.8 sürümünü kullanmaktadır. Sistem, bayi operasyonlarını yönetmek için kritik öneme sahip birçok modül içermektedir. Tabii Microsoft'un .Net Framework için olan desteği 2029 yılında sona erecektir. Bu nedenle, sistemin gelecekteki sürdürülebilirliği için modernizasyon kaçınılmazdır.

Genel olarak N-Tier mimarisine göre düzenlenmiş bir sistemdir. Presentation, Business Logic Layer ve Data Acces Layer olmak üzere üç ana katmandan oluşan bir mimari üzerine kurgulanmıştır. Daha önceden var olan Façade katmanı ilk modernizasyon çalışması kapsamında kaldırılmıştır. Sistem Microsoft SQL Server veritabanı kullanmaktadır. İş kuralları ve süreçleri modül bazında son derece karmaşık ve iç içe geçmiş olabilir. Bu noktada SQL Server'ın stored procedure avantajları gözetilerek iş kuralları ve süreçlerin bir kısmı veritabanı katmanında da uygulanmıştır. Dolayısıyla kod ve veritabanına yayılmış iş kuralları ve süreçleri mevcuttur.

Onlarca yıllık bir uygulama söz konusu olduğundan beş milyon satırdan fazla bir kod tabanı söz konusudur. Binlerce ekran, yüzlerce stored procedure, tera baytlarca veri, onlarca servis beş ana modül etrafında birleşir. Bu modüller finansal hizmetler, araç, satış sonrası hizmetler, yedek parça, müşteri olarak sıralanabilir. Sistem, yüzlerce bayi tarafından kullanılmakta olup, milyonlarca müşteriye hizmet vermektedir.

Sistemle entegre çalışan birçok uygulama da vardır. Örneğin ayrı bir raporlama sistemi bulunmaktadır. Bu sistem datawarehouse mimarisi üzerine kurulmuş olup, ETL süreçleriyle ana sistemden verileri çekmektedir. Raporların hazırlanması için planlanmış işler kullanılır. 200den fazla Job vardır ve bunların bazılarının çalışma süresi saatler mertebesindedir. Job'ların çoğu doğrudan Stored Procedure işletmekle kimisi de Microsoft'un SSIS *(Sql Server Integration Services)* hizmetleri şeklinde çalıştırılmaktadır. Bazı raporlar anlık üretilebilen türdedir ve bunlar için Liquid rapor şablonları kullanılmaktadır. Daha önceki dönemlerde Microsoft'un SSRS *(Microsoft SQL Server Reporting Services)* raporları da kullanılmıştır.

Sistem aynı zamanda regülasyonlar içeren dış servislere de bağımlılıklar içerir. Örneğin, elektronik faturlama ve irsaliye sistemleri, POS tabanlı ödeme cihazları, kurum için yazılmış yeni nesil uygulamalar vb. Bu sistemlerde haberleşme için ağırlıklı olarak SOAP ve REST tabanlı web servisleri kullanılmaktadır. Ana sistemden dışarıya açılan fonksiyonellikler içinse  XML Web Servisler ve WCF servisleri kullanılmaktadır. Ayrıca yeni nesil uygulamaların ihtiyaç duyduğu veya karşılıklı olarak dahil olunması gereken süreçlerde asenkron mesajlaşma altyapısı kullanılmaktadır. Bunun için Rabbit MQ tercih edilmiştir. Modüller de kendi aralarında kullandığı ortak süreçlere sahiptir. Ortak ve tek bir veritabanı sistemi olduğundan modüller arası veri paylaşımı doğrudan veritabanı katmanından ve iş nesneleri üzerinden yapılmaktadır.

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

### Teknoloji Altyapısı

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

Modernizasyon ihtiyaçlarının netleştirilmesi için 2020 öncesinde birçok **fizibilite** çalışması gerçekleştirilmiş ve var olan durum detaylı raporlarla açıklanmıştır. Yeni mimari modellere geçmek ve modüllerin bağımsız olarak çalıştırılabilmesi stratejik hedef olarak belirlenmiştir. Bu kapsamda ilk uzun soluklu **IT4IT** çalışması 2020 yılında başlatılmıştır. Bu çalışmada bir yol haritası çıkartılmış ve aşağıdaki ana adımlar atılmıştır.

- **Sonarqube** ile kod kalitesinin düzenli olarak ölçümlenmesi ve raporlanması.
- **Teknik borçlar** 1000 kişi gün maliyetinden 100 kişi gün altına düşürülmüş yer yer sıfıra indirilmiştir.
- **Façade** katmanı kaldırılmıştır.
- **CBL** katmanındaki fonksiyonellikler soyutlanmış ve ayrı bir katmana taşınmıştır.
- Tüm bileşenler için **dependency injection** altyapısı kurulmuştur *(Windsor Castle)*.
- **Unit testler** yazılmaya başlanmış ve **code coverage** değerlerinin kabul edilebilir seviyelere gelmesi sağlanmıştır.
- **CI/CD** süreçleri iyileştirilmiş ve otomasyon oranı artırılmıştır.

Bu modernizasyon çalışmaları hali hazırda devam etmektedir ancak asıl stratejik hedeflere ulaşma noktasında ürünün sıfırdan yazılma maliyetinin çok yüksek olması nedeniyle yeni yaklaşımlar araştırılmaya başlanmış ve bu kapsamda yapay zeka tabanlı modernizasyon çözümleri mercek altına alınmıştır. Yazının bundan sonraki kısımlarında son altı aylık dönem içerisinde gerçekleştirilen yapay zeka tabanlı modernizasyon çalışmaları anlatılmakta olup sonuçlar ve gelecek planları paylaşılmaktadır. 

### Motivasyon

Sistemin karmaşıklığı, büyüklüğü ve kritik iş süreçlerini içermesi nedeniyle geleneksel modernizasyon yöntemleriyle ilerlemek çok uzun sürecek ve yüksek maliyetli olacaktır. Yapay zeka tabanlı modernizasyon çözümleri, kod analizi, otomatik refaktörizasyon, test otomasyonu ve hatta kod üretimi gibi alanlarda önemli avantajlar sunarak bu süreci hızlandırabilir ve maliyetleri düşürebilir. Ayrıca, yapay zeka destekli araçlar, kodun karmaşıklığını daha iyi anlayarak teknik borçları tespit edebilir ve önceliklendirebilir, böylece modernizasyon sürecini daha etkili hale getirebilir.

### Riskler

Yapay zeka tabanlı teknolojilerin gelişimi ve vaat ettikleri çok cazip görünse de büyük çaplı ve karmaşık kurumsal çözümlerde ele alınmasının bir **PoC** *(Proof of Concept)* çalışmasıyla başlaması ve sonuçların dikkatle değerlendirilmesi gerekmektedir. Bu kapsamda aşağıdaki riskler göz önünde bulundurulmalıdır:

- Yapay zeka tabanlı araçların kodun karmaşıklığını tam olarak anlayamaması ve kritik iş süreçlerini doğru şekilde analiz edememesi.
- Otomatik refaktör işlemlerinde hatalı düzenlemeler önermesi.
- Kaynak olarak kullanılan bilgilerin veri sızıntısına neden olabilmesi.
- Halusinasyon sebebiyle yanlış önerilerde bulunması.
- İnsan denetimi olmadan yapılan değişikliklerin beklenmedik sonuçlara yol açması.
- Yapay zeka tabanlı araçların mevcut kod tabanıyla entegrasyon sorunları yaşaması.
- Yapay zeka tabanlı araçların öğrenme sürecinde zaman alması ve başlangıçta düşük performans göstermesi.

## Level 0: Birinci Aşama

İlk etapta bir **PoC** çalışması ile başlanmasına karar verilmiş ve belli bir modülün orta karmaşıklıkta iş süreçleri barındıran bir alt bölümünün sıfırdan, lisansı alınmış yapay zeka modelleri kullanılarak yeniden geliştirilmesine karar verilmiştir. Ağırlıklı olarak **Anthropic**'in **Claude Sonnet** modeli tercih edilmiştir. Bunun en büyük sebebi diğer modellere göre daha tutarlı kodlar üretmesi ve **halüsinasyon** oranının daha düşük olduğunun gözlemlenmesidir. Süreçte **front-end** tarafında **Vue** ve **Nuxt**, **back-end** tarafında ise **.NET Core** kullanılmasına karar verilmiştir. Özellikle ön yüz tarafında kurum için geliştirilen özel komponentler tercih edilmiştir. Veri tabanı tarafında **PostgreSQL**'de karar kılınmış ve kod tarafında **Entity Framework** ile **Dapper** entegrasyonları tercih edilmiştir. **Authentication/Authorization** için halihazırda diğer yeni nesil kurum içi uygulamaların da kullandığı servisler tercih edilmiş ve **Keycloak** ile devam edilmiştir. Kod tabanı **GitHub**'a alınmış ve **CI/CD** hattında **GitOps** kullanılarak otomatikleştirilmiştir. Kod kalitesi ve güvenlik taramaları için **Sonarqube** entegre edilmiştir. **Backend** taraf ile **front-end** arası haberleşme yine **REST API** üzerinden sağlanmış ancak özel entegrasyon noktaları için gerekli soyutlamalar da yapılmıştır. Bu sayede örneğin **gRPC** tabanlı noktalarla entegre olunabilmiştir. **Backend** tarafta kurum içi geliştirilmiş ve **cross-cutting concern**'leri de ele alan bir framework kullanılmıştır. Burada bağımlılıkların yönetimi için **.NET**'in dahili **dependency injection** altyapısı kullanılmıştır. Yeni yazılan **PoC** uygulamasında **legacy** sistemden hiçbir parçanın yer almamasını ve her şeyin sıfırdan tasarlanmasına özellikle dikkat edilmiştir.

### Geliştirme Süreci

Geliştirme sırasında ağırlıklı olarak **Visual Studio Code** kullanılmıştır. Doğrudan **Copilot** chat penceresi üzerinden ilerlemek yerine, yazılması istenen parçalar için **markdown** belgeleri hazırlanarak ilerlendi. Öncelikli model olarak **Claude Sonnet** kullanıldı, ancak bazı durumlarda **GPT**, **Gemini** ve **Grok** ile de kıyaslamalar yapıldı. Burada özellikle deneysel aşamada olan modeller şirket verilerinin gizliliğini korumak için tercih edilmedi. Geliştirme sürecindeki safhaları aşağıdaki gibi ana hatlarıyla özetleyebiliriz:

- **Spec-Oriented** yaklaşımı benimsendi ve yapay zeka asistanlarına kullandırılan dokümanlar hazırlandı.

```text
- docs
  - architectural-overview (Sistemin genel mimari yapısı, kullanılan teknolojiler, kodlama standartları, bileşene ait rehber dokümanlar, çoklu dil desteği için dokümanlar vb yer alır)
  - business (Burada feature baslı user story'ler yer alır)
  - domain-model (DDD kurgusunda entity, value object, aggregate root dokümanları ile süreç elemanları özetlenir)
  - ui (mock-up ekranlarının HTML formatlı hallerinin yer aldığı klasördür)
  - static-data (Sabit veriler, parametreler vb için dokümanlar yer alır)
  - prompts (uçtan uca API üretme, EF migration hazırlama ve işletme, uçtan uca Vue sayfası oluşturma gibi işlemler için yapay zeka asistanlarına kullandırılan prompt'ların yer aldığı klasördür)
```

- Yapay zeka asistanları ile etkileşim için belirli **Copilot** üzerinde **Agent**'lar tanımlandı: Senior Software Developer, UI/UX Expert, Senior Business Analyst, DevOps Engineer, QA Engineer gibi.
- Diğer modüllerin kolayca geliştirme yapmaya başlamaları için bir **dotnet template** projesi ve **CLI** aracı geliştirildi. Bu sayede sıfırdan bir projeye başlayacaklar için gerekli spec doküman şablonlarını içeren, çalışır temel **back-end** ve **front-end** uygulamalarını otomatik olarak oluşturan araçlar sağlandı.
- **Domain** odaklı geliştirilmiş **Framework** ve **Source Code Generator** kütüphaneleri kurum içi **NuGet** repolarına benzer şekilde **Nuxt** bileşenleri de **npm** repolarına alındı.
- Üretilen çözüm alt yapısı belirli bir olgunluğa ulaştıktan sonra, kod kalitesi ölçümü için **Sonarqube** ile entegrasyon sağlandı. Ayrıca **SonarSource Sonarqube MCP Server** ile entegre olundu ve **VS Code** arabiriminden çıkmadan, yerleşik agent'lar yardımıyla, bulgu analizi, yorumlama, düzeltme *(issue çözdürme, cognitive complexity düşürme, code-coverage değerlerini yükseltme)* gibi işlemler yapıldı.

### Deneyimler

Çalışma sırasında elde edilen deneyimlerimizi aşağıdaki gibi özetleyebiliriz.

- Yapay zeka asistanları ile etkileşimde doğru **prompt**'ların hazırlanması ve sürekli iyileştirilmesi kritik öneme sahip. Başlangıçta hazırlanan prompt'lar yeterince spesifik olmadığında, üretilen kodlar beklenen kaliteye ulaşmamış ve manuel müdahale ihtiyacı artmıştır. Bu nedenle **spec** dokümanlarının detaylandırılması ve örnek kod parçalarının sunulması önemli bir rol oynamıştır.
- Yapay zeka asistanlarının ürettiği kodların kalitesi, modelin eğitildiği veri setine ve modelin kapasitesine bağlı olarak değişiklik göstermektedir. Bu nedenle, farklı modellerin karşılaştırılması ve en uygun olanın seçilmesi gerekliliği ortaya çıkmıştır.
- Kod üretimi ve analiz çıkartılması **Copilot** ajanlarına alındıktan sonra daha tutarlı sonuçlar elde edildiği gözlemlendi. Taleplerin yeni **feature branch**'lerde oluşturulması, **review** için insana gönderilmesi, review sırasında verilen yorumlara karşılık ek düzeltmeler yapılması ve **Pull Request** süreci işletilerek ilerlenmesi daha güvenli hissetmemizi sağladı.
- Özellikle **domain** içerisinde yer alacak **entity**, **value object**, **aggregate root** gibi yapıların doğru şekilde modellenmesi ve **spec** olarak dokümante edilmesi, belli standartlar çerçevesinde kod üretilmesi açısından faydalı oldu. Yapay zeka asistanları bu ilişkilerden yararlanarak genel senaryoları oluşturmakta *(örneğin, araç siparişi oluşturma, müşteri şikayeti alma, vb)* daha başarılı oldular.
- Küresel bir standartta olmayan bazı alanlarda detaylı **spec** dokümanları olmadığında tutarlı sonuçlar elde etmekte zorlandık. Örneğin özel bileşenlerden oluşan **UI** kütüphanelerinde, yapay zeka asistanlarının doğru kod parçalarını üretmesi için detaylı örnekler ve kullanım şekilleri sunmak gerekti. Bu amaçla bileşen setleri için nasıl kullanıldığına dair dokümanlar hazırlandı ve örnek kod parçaları sunuldu. Bu aslında bir **RAG** *(Retrieval Augmented Generation)* yaklaşımı için de bize yol gösterici oldu. *(RAG yaklaşımı ile domain'e özgü bilgi ve dokümanların yapay zeka asistanlarına sunulması, daha doğru ve tutarlı sonuçlar elde edilmesini sağlayabilir)*
- **Sonarqube** ilk bulguları agent bazlı geliştirmelerde kod tabanları hatasız derlense de bir takım teknik borçların ortaya çıktığını gösterdi. Dolayısıyla insan denetimi ve müdahalesi olmadan tamamen hatasız bir sürecin işletilmesinin şu an için mümkün olmadığı gözlemlendi. Ancak, **Sonarqube MCP Server** ile entegrasyon sayesinde, yapay zeka asistanlarının bu bulguları analiz ederek düzeltme önerileri sunması ve hatta bazı düzeltmeleri otomatik olarak yapması sağlandı. Bu da sürecin hızlanmasına ve kod kalitesinin artırılmasına katkıda bulundu.

### Çalışma Sırasında Arada Yazılan Yardımcı Araçlar

Bu çalışma sırasında geliştirme hızımızın önemli ölçüde arttığını fark ettik ve birkaç yardımcı araç da yazdık:

- **Domain Sözlüğü:** Ürün paydaşlarının ortak bir terminoloji kullanmasını sağlamak için bir domain sözlüğü aracı geliştirdik. Bu araç, yapay zeka asistanlarının doğru terimleri kullanmasını kolaylaştırdı.
- **MCP** *(Model Context Protocol)*: Template kullanımı haricinde kullanıcı hikayeleri *(User Story)* ve iş kurallarını barındıran analiz dokümanlarını minimum hatada oluşturmak için bir **MCP** sunucusu ve gerekli **API** endpoint'leri geliştirildi. İlgili **MCP server** **VS Code** ortamlarına da adapte edildi ve böylece analist veya yazılım geliştiricilerin, yapay zeka asistanlarıyla bu **domain** özelinde konuşabilmeleri ve domain kurallarına uygun çıktılar alabilmeleri sağlandı.
- **Tersine Mühendislik Aracı:** Var olan **legacy** sistemdeki belirli kod parçalarının analiz edilerek, iş kurallarının ve süreçlerin çıkarılmasını sağlayan bir tersine mühendislik aracı geliştirildi. Bu araç ile var olan bazı iş kuralları için doküman hazırlanması, gözden geçirilmek kaydıyla yeni sisteme adapte edilmeleri ile ilgili test ortamları sağlanmış oldu.

### Teknik Özet

Yukarıdaki süreçte kullanılan başlıca teknolojiler ve araçlara ait özet bilgileri aşağıdaki tabloda bulabilirsiniz:

| Kategori                   | Teknoloji / Araç                                         |
|----------------------------|----------------------------------------------------------|
| Yapay Zeka Modelleri       | Anthropic Claude Sonnet, OpenAI GPT, Google Gemini, Grok |
| YZ Asistanları             | GitHub Copilot                                           |
| Metodoloji                 | Spec-Oriented Development, RAG *(Deneme Aşamasında)*     |
| Front-End Teknolojileri    | Vue.js, Nuxt.js                                          |
| Back-End Teknolojileri     | .NET Core, C#                                            |
| Veri Tabanı                | PostgreSQL                                               |
| ORM                        | Entity Framework, Dapper                                 |
| Auth/Authorization         | Keycloak                                                 |
| CI/CD                      | GitHub Actions                                           |
| Kod Kalitesi ve Güvenlik   | Sonarqube *(MCP Server ile birlikte)*, Copilot           |

## Sonuçlar

Bu çalışma kapsamında elde edilen başlıca sonuçlar aşağıdaki gibi özetlenebilir:

- Yapay zeka tabanlı modernizasyon süreci, geleneksel yöntemlere kıyasla geliştirme süresini %40 oranında azalttı.
- Üretilen kodların kalitesi, manuel olarak yazılan kodlarla karşılaştırıldığında benzer seviyelerde bulundu.
- Teknik borçların azaltılması ve kodun daha modüler hale getirilmesi sağlandı.
- Yapay zeka asistanlarının doğru şekilde yönlendirilmesi ve etkileşimde bulunulması, sürecin başarısında kritik rol oynadı.
- Ancak, bazı durumlarda yapay zeka asistanlarının ürettiği kodların gözden geçirilmesi ve manuel müdahale gerektirdiği görüldü.
- Şu an ve yakın vadede mutlak suretle insan denetimli bir sürecin işletilmesi gerektiği gözlemlendi.

## Sonraki Planlar ve Hedefler

- **PoC** çalışmasının başarıyla tamamlanmasının ardından, yapay zeka tabanlı modernizasyon sürecinin diğer modüllere de genişletilmesi planlanmakta.
- Yapay zeka asistanlarının eğitiminde kullanılacak veri setlerinden hareketle daha küçük ve **domain**'e özgü dil modelleri *(Custom LLM)* oluşturulması değerlendirilebilir.
- **RAG** *(Retrieval Augmented Generation)* yaklaşımının daha etkin kullanılması için doküman yönetim sistemlerinin entegrasyonu planlanabilir.
- Süreçte kullanılan **prompt**'ların sürekli iyileştirilmesi ve optimize edilmesi için bir geri bildirim mekanizması oluşturulması faydalı olacaktır.
- **MCP** tabanlı etkileşimlerin daha da geliştirilmesi ve yaygınlaştırılması düşünülebilir.
- Mutlak suretle çıktıların maliyeti ölçümlenmeli ve insan denetimi için gereken kaynaklar optimize edilmelidir.

---

## Demo Simülasyonu

Bu repoda, sunumda anlatılan **Spec-Oriented Development** yaklaşımını göstermek için basitleştirilmiş bir **Araç Envanter Yönetimi** modülü simülasyonu bulunmaktadır.

### 📁 Doküman Yapısı

```
docs/
├── architectural-overview/     # Teknoloji stack, kodlama standartları, proje yapısı
├── business/                   # User story'ler (US-001, US-002)
├── domain-model/               # Entity ve Value Object tanımları
├── ui/                         # HTML mockup'lar
├── static-data/                # Enum'lar, marka/model listeleri
└── prompts/                    # AI asistanlarına kullandırılacak prompt'lar
```

### 🎯 İçerik

- **2 User Story**: Araç ekleme ve listeleme
- **3 Domain Model**: Vehicle (Entity), VIN ve Money (Value Objects)
- **2 UI Mockup**: Araç ekleme formu ve liste sayfası
- **3 AI Prompt Template**: API endpoint, EF Migration, Vue component geliştirme
- **Coding Standards**: C# ve TypeScript/Vue için detaylı standartlar

### 🚀 Kullanım

Bu dokümanlar **GitHub Copilot** veya diğer **AI** asistanlarına context olarak verilerek:
1. Backend API endpoint'leri geliştirilebilir
2. Database migration'ları oluşturulabilir
3. Frontend component'leri üretilebilir
4. Test senaryoları yazılabilir

Detaylı kullanım için: [`docs/prompts/README.md`](docs/prompts/README.md)

### 📝 Not

Bu simülasyon **eğitim ve sunum amaçlıdır**. Gerçek kurumsal uygulamanın kod ve verileri gizlilik nedeniyle paylaşılamamıştır.
