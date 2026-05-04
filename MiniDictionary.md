# Sözlük

[Readme](README.md) dokümanında geçen genel terimlerin toplandığı mini sözlük.

## Araçlar ve Platformlar

- **Azure DevOps**: Microsoft'un DevOps için sunduğu bulut platformu *(CI/CD pipeline, git repo, boards)*
- **Docker / Docker Compose**: Uygulama ve altyapı servislerini konteyner olarak paketleyen ve çalıştıran platform; bu projede tüm altyapı Docker Compose ile ayağa kaldırılır
- **Git Flow**: main, develop, feature, release ve hotfix dallarından oluşan git branching stratejisi
- **GitHub Actions**: GitHub'ın yerleşik CI/CD otomasyon servisi
- **GitHub Copilot**: GitHub ve Microsoft'un geliştirdiği, kod yazma ve görev icrasında yardımcı olan AI asistanı
- **LM Studio**: Büyük dil modellerini yerel makinede çalıştırmaya yarayan masaüstü uygulaması; bu projede RAG için OpenAI uyumlu API endpoint'i olarak kullanıldı. Setup ve kurguyu çalıştırmak için ideal.
- **NuGet**: .NET için paket yöneticisi. Ne ararsan var yahu :D
- **pgAdmin**: PostgreSQL için web tabanlı yönetim arayüzü ama DBeaver'a bir bakın derim.
- **Playwright**: Uçtan uca (E2E) web test otomasyonu için kullanılan araç. Şöyle bir taktiğim var. Feature ekranlarına ait playwright testlerini yapay zekaya yazdırıp, çıkan bulguları da ona çözdürme yoluyla *(Tabii bizzati review ediyorum)* kabul kritlerini daha kısa sürede kontrol ettiriyorum. 
- **RabbitMQ**: Açık kaynaklı message broker; servisler arası asenkron mesajlaşma için kullanılır. Eğer sistemde olay bazlı haberleşmeler olacaksa sanırım ilk aklıma gelen çözüm oysa ki değer mi belki Redis Pub/Sub ile daha hızlı çözeceksin, belki event bazlı bir şey yapmasan da olur. Bunlar hep detaylıca düşünülmesi gereken şeyler :D
- **SonarQube**: Sürekli kod kalitesi ve güvenlik analizi platformu olarak bilinen statik kod tarayıcı. bug, code smell, güvenlik açıkları hatta kuş uçsa haberi olur.
- **Vitest**: Vite ekosistemi için geliştirilmiş hızlı birim test framework enstrümanı
- **Visual Studio Code (VS Code)**: Microsoft'un geliştiriciler arasında yaygın olarak kullanılan hafif ve genişletilebilir kod editörü
- **Windsor Castle**: .NET için IoC container ve Dependency Injection framework'ü *(legacy projede kullanılmaktaydı)*
- **Yarn**: JavaScript/Node.js için hızlı ve güvenilir paket yöneticisi. Bu projede npm yerine tercih ettim zira Windows'ta biraz sıkıntı çıkardı.

## Bazı Teknik Terimler

- **Acceptance Criteria (Kabul Kriterleri)**: Bir user story'nin tamamlandığının doğrulanması için gereken ölçülebilir koşullar
- **ADO.NET**: Microsoft'un efsane veri erişim teknolojisinin kullanıdığı çatı kütüphaneler topluluğu desek yeridir. Veritabanı işlemleri için düşük seviyeli API fonksiyonellikleri sağlar. Aslında **Dapper**'ın, onun çok daha mükelleştirilmiş bir hali olduğunu düşünüyorum.
- **Aggregate Root**: Domain Driven Design terminolojisine göre bir entity kümesinin kök elemanıdır ve tutarlılık sınırlarını belirler.
- **ASP.NET Web Forms**: Microsoft'un yine efsane yapılarından birisidir. Sunucu bazlı event-driven web uygulama geliştirme framework'ü olarak özetleyebiliriz. Tabii tam bir *(legacy)* sistem parçasıdır.
- **Bootstrap 5**: Kullanıcı deneyimi yüksek ve mobil-öncelikli web arayüzleri geliştirmek için kullanılan CSS/JS framework. Gayet kullanışlı, sade ve uzun zamandır ayakta kalmış bir çatı.
- **Clean Architecture**: Bağımlılıkların dışarıdan içeriye doğru aktığı, Core, Infrastructure ve Presentation gibi temel katmanları içeren soğan halkası mimarilerinden birisi.
- **Claude Sonnet**: Anthropic firması tarafından geliştirilmiş olan büyük dil modeli *(Large Language Model)*. Onu en çok kod üretimi noktasında değerlendirdik.
- **CQRS (Command Query Responsibility Segregation)**: Okuma *(Query)* ve yazma *(Command)* işlemlerini birbirinden ayıran yazılım prensipleri bütünü mü desek.
- **Code Coverage**: Test kodlarının kaynak kodun ne kadarını çalıştırdığını gösteren yüzdesel metrik
- **Cognitive Complexity**: Kodun anlaşılma zorluğunu ölçen SonarQube metriği
- **Cross-Cutting Concerns**: Loglama, security, transaction yönetimi gibi birçok katmanı ilgilendiren ortak fonksiyonelliklere verilen genel isimlendirme. Kıstas bunlar :D
- **DevOps**: Yazılım geliştirme *(Dev)* ve IT operasyonlarını *(Ops)* birleştiren bir kültür, pratikler ve araçlar kümesi.
- **Dapper**: .NET için hafif, yüksek performanslı bir micro-ORM aracı. Az önce demiştim bence Ado.Net'in mükemmelleştirilmiş hali.
- **DDD (Domain-Driven Design)**: İş mantığını ve domain dilini merkeze alan yazılım tasarım yaklaşımı. Anlaması zor, anlatması zor, uygulaması zor ama başardın mı tadından yenmez.
- **Dependency Injection**: Bileşenler arası bağımlılıkların nesne içinde oluşturulmak yerine dışarıdan enjekte edildiği tasarım ilkesi.
- **Entity**: DDD'de benzersiz kimliği olan ve yaşam döngüsü takip edilen iş nesnesi.
- **Entity Framework Core (EF Core)**: Microsoft'un .NET için geliştirdiği ORM framework enstrümanı.
- **ETL (Extract, Transform, Load)**: Verilerin çekilmesi, dönüştürülmesi ve hedef sisteme yüklenmesi sürecini ifade eder.
- **FluentValidation**: .NET'te güçlü ve okunabilir doğrulama kuralları yazmak için kullanılan kütüphanedir.
- **GitOps**: Git repository'sini hem uygulama kodu hem de altyapı konfigürasyonu için tek doğruluk kaynağı olarak kullanan yaklaşım.
- **gRPC**: Google tarafından geliştirilen yüksek performanslı, çift yönlü iletişim dahil HTTP/2nin nimetlerini pek bir güzel kullanan standartlar bütünü.
- **Halüsinasyon**: Yapay zeka modellerinin gerçek olmayan veya yanlış bilgi üretmesi. İşte bunun önüne geçmek için Vector RAG, Graph RAG, MCP, Cached-Context Windowi Fine-Tuning vs *(Kendimiz baştan yazalım daha iyi)*
- **Hot Module Replacement (HMR)**: Geliştirme sırasında sayfayı yenilemeden değiştirilen modüllerin anında güncellenmesini sağlayan Vite özelliği
- **Keycloak**: Açık kaynaklı Identity ve Access Management *(IAM)* çözümü. Kurulumu biraz zor ama yönetimi bence daha da zor :D
- **Legacy System**: Eski teknoloji veya mimarilerle geliştirilmiş, uzun süredir çalışan sistemler. Modernizasyon için AI bir fırsat olabilir mi? Cevaplar bu sunumda ya da sunumda idi ;)
- **Liquid Template**: Shopify tarafından geliştirilen güvenli ve esnek bir template *(şablon)* dili; rapor sayfalarının üretiminde sıklıkla kullanıyoruz.
- **MCP (Model Context Protocol)**: AI modellerinin harici araçlara ve veri kaynaklarına standart bir arayüz üzerinden erişmesini sağlayan açık kaynak protokol.
- **MediatR**: .NET'te CQRS implementasyonu için kullanılan, gevşek bağlı *(loosely coupled)* mesajlaşma kütüphanesi
- **Microsoft Semantic Kernel**: Microsoft'un AI orkestrasyonu için geliştirmiş olduğu SDK. LLM entegrasyonu, RAG ve agent senaryoları için ideal. Bu çalışmada bolca kullandık.
- **N-Tier Architecture**: Layered Architecture örneklerinden olan genellikle Presentation, Business Logic, Data Access gibi üç katmanla ifade edilen yazılım mimarisi
- **ORM (Object-Relational Mapping)**: Nesne modeli ile ilişkisel veritabanı tabloları arasında dönüşüm sağlayan mekanizma. Adı üstünde zaten.
- **PoC (Proof of Concept)**: Bir fikrin veya yaklaşımın uygulanabilirliğini test etmek için yapılan deneysel çalışma. Sonuçlar beklendiği gibi ise bu PoC'yi doğrudan üretime almamalıyız. Yeni bir çözümde gidip PoC' deki tecrübeleri tekrardan refactor ederek yerleştirmeliyiz. İçimde kalmadan söylemek istedim :D
- **PostgreSQL**: Açık kaynaklı, güçlü bir ilişkisel veritabanı yönetim sistemi. Sıklıkla yüksek lisans maliyetleri nedeniyle gündeme gelen veritabanlarının yerine ikame edilmeye çalışılır.
- **Qdrant**: Yüksek performanslı, açık kaynaklı vektör veritabanı; RAG senaryolarında embedding'lerin saklanması ve aranması için kullanılır ve sıkık durun **Rust programlama dili ile yazılmıştır**
- **RAG (Retrieval Augmented Generation)**: AI modeline yanıt üretmeden önce ilgili doküman parçalarını vektör veritabanından çekip bağlam olarak sunan yaklaşım. Ama sadece bu kadarla kalsa... Vector RAG, Graph RAG, Agentic RAG :D
- **Serilog**: .NET için yapılandırılmış *(structured)* loglama kütüphanesi.
- **Skill (Copilot Skill)**: GitHub Copilot Agent'larına bağlı, belirli bir görevi nasıl yürüteceğini tanımlayan yeniden kullanılabilir talimat dosyası
- **SSIS (SQL Server Integration Services)**: Microsoft'un ETL ve veri entegrasyonu aracı. Özellikle planlı işlerde sıklıkla değerlendirilen güçlü bir enstrüman.
- **SSRS (SQL Server Reporting Services)**: Microsoft'un kurumsal raporlama platformu
- **stdio Transport**: MCP'de standart giriş/çıkış akışları üzerinden iletişim kuran transport modu; ayrı port gerektirmez. Ancak üretim sistemlerinde Streamable HTTP tercih edin derim ki Server-Sent Events'i de otomatik olarak sağlar ama ondan daha iyidir.
- **Stored Procedure**: Veritabanında saklanan ve çalıştırılabilen SQL kod blokları. Performanslı ve domain odaklı oldukları için tercih edilirler ama içinde With(Nolock) sız bir select yaptınız mı koca armadayı kilitlemeniz içten bile değildir.
- **TypeScript**: JavaScript'in tip güvenli üst kümesi olarak ifade edilebilir. Derleme zamanında hata yakalamayı sağlar. Ben pek ısınamadım ama olsun.
- **Value Object**: DDD'de kimliği olmayan, yalnızca taşıdığı değerle tanımlanan nesneler *(örneğin `Money`, `VIN`, `Email` ki bu çalışmada kullandık)*
- **Vite**: Yeni nesil frontend build aracı. Son derece hızlı HMR ve modern ES modülleri desteği de sunar
- **Vue.js 3**: Progressive JavaScript framework olarak karşımıza çıkıyor. Composition API ve TypeScript ile modern kullanıcı arayüzleri geliştirmek için kullanılır lakin 2de geliştirdiğimiz bileşenleri 3e çıkarırken ne ağladık birde bize sorun.
- **WCF (Windows Communication Foundation)**: Microsoft'un servis tabanlı uygulamalar için SOA framework'ü *(legacy)*
- **xUnit**: .NET için modern, genişletilebilir birim test framework'ü. Birim test yazın arkadaşlar, yazın!

## Diğer Terimler

- **Bayi Yönetimi Sistemi (DMS)**: Otomotiv sektöründe bayi operasyonlarını *(satış, servis, yedek parça, finans)* yöneten kapsamlı yazılım sistemi
- **Domain**: Yazılımın çözdüğü iş probleminin belirli alanı ve konusu
- **Feature Branch**: Yeni özellik geliştirmek için ana geliştirme dalından açılan izole alan. Genelde tüm özellikleri feature branch'te yazar, test eder, test ettirir, sonra merge etmek için PR'a alırız.
- **INVEST**: Geçerli bir user story'nin sahip olması gereken 6 özellik — Independent, Negotiable, Valuable, Estimable, Small, Testable
- **Modernizasyon**: Eski sistemlerin yeni teknoloji, mimari ve geliştirme pratiklerine taşınması süreci
- **Opsiyon (Vehicle Option)**: Bir aracın belirli bir müşteri adına belirli bir süreliğine rezerve edilmesi işlemi
- **Pull Request**: Kod değişikliklerinin incelendikten sonra ana dal ile birleştirilmesi için yapılan istek
- **SDLC (Software Development Life Cycle)**: Yazılımın planlama, analiz, tasarım, geliştirme, test ve dağıtım aşamalarını kapsayan yaşam döngüsü
- **Spec-Oriented Development**: Geliştirmeye başlamadan önce domain, mimari ve UI'yi detaylı markdown dokümanları ile tanımladıktan sonra bu dokümanları AI asistanlarına bağlam olarak sunarak ilerlenen geliştirme yaklaşımı
- **Sprint**: Agile metodolojide sabit sürede planlanan çalışma periyodu (genellikle 1–4 hafta)
- **Technical Debt (Teknik Borç)**: Hızlı çözümler veya eksik standartlar nedeniyle birikmiş, ilerleyen dönemde düzeltilmesi gereken kod kalitesi ve mimari sorunlar
- **Ubiquitous Language**: DDD'de geliştirici, iş analisti ve paydaşların ortak kullandığı, kodda da yansıtılan domain sözlüğü
- **User Story**: Kullanıcı bakış açısından yazılmış, bir iş değeri sunan fonksiyonel gereksinim birimi
- **VIN (Vehicle Identification Number)**: ISO 3779 standardında tanımlanmış, bir aracı küresel olarak benzersiz şekilde tanımlayan 17 karakterlik kod
