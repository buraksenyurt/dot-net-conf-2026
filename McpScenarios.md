# MCP Senaryoları

Bu dokümanda MCP Server örneğine ait iki senaryo gösterilmektedir. Bu senaryolar uygulamanın iş süreçlerine dair fonksiyonellikleri ele alır. Deneysel bir çalışma olduğundan VS Code arabirimi kullanılmıştır. Gerçek hayat senaryosunda ayrı bir istemci uygulama üzerinden bu senaryoların işletilmesi beklenir.

## Senaryo 1: Var Olan Bir Müşteri İçin Opsiyonlama

Bu senaryoda sistemde kayıtlı olan bir müşteri için yine sistemde var olan araçlardan birisinin satın alma opsiyonlu olarak ayrılması söz konusudur. Yetkili personel *(ki bu denemede VS Code kullanan geliştirici :D)* şöyle bir prompt girer.

```text
Burak Selim için yeni bir araç opsiyonlamak istiyorum.
```

Buna göre VS Code sorulan soruyla ilgili bir MCP server olup olmadığına bakar ve ilgili aracı keşfettikten sonra olaylar aşağıdaki gibi gelişir.

Vekil ajan müşteri bilgisini ilgili MCP tool üstünden arar ve bulur. Ayrıca araç listesinden opsiyonlanabilecek olan araçların bir listesini seçilmek üzere getirir.

![Mcp Runtime 00](./images/mcp_runtime_00.png)

Opsiyonlanacak araç seçimi sonrası bununla ilgili aracın çalıştırılması için bir izin de istenir.

![Mcp Runtime 01](./images/mcp_runtime_01.png)

Bu sırada tool ile gelen bir eksik fark edilir. Opsiyonlama için süre de belirtilmelidir. Bunun üzerine vekil ajan tekrar devreye girer ve bu bilgiyi de sorar. Vay arkadaş.

![Mcp Runtime 02](./images/mcp_runtime_02.png)

Hatta opsiyonlama için birde kapora ücreti gerekir. Bedavaya araç verecek değiliz :D Ki bu da vekil ajanın keşfettiği tool'un bir paramtresidir. O da sorulur ve eklenir.

![Mcp Runtime 03](./images/mcp_runtime_03.png)

![Mcp Runtime 04](./images/mcp_runtime_04.png)

Burada vekil ajanın yetki talebine tüm session boyunca izin verilmesi de gündemdedir ama kontrol emin olana kadar sizde olsun derim ;)

![Mcp Runtime 05](./images/mcp_runtime_05.png)

## Senaryo 2: Yeni Bir Müşteri İçin Araç Opsiyonlama

Bu seferki senaryoda ise sistemde kayıtlı olmayan bir müşteri için araç opsiyonlama süreci ele alınır. MCP server'ın bu seferki görevi müşteri kaydı oluşturmak ve ardından araç opsiyonlama sürecini işletmektir. Yine yetkili personel *(Kahramanımız VS Code geliştiricisi :D)* aşağıdaki prompt'u girer.

```text
Spidi Gonzalez için bir araç opsiyonlamak istiyorum.
```
