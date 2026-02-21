-- =============================================================================
-- seed-data.sql
-- Demo verisi — VehicleInventory uygulaması
--
-- Kullanım (PowerShell):
--   Get-Content scripts/seed-data.sql | docker exec -i aio-postgres psql -U johndoe -d VehicleInventory
--
-- Kullanım (Linux/macOS):
--   docker exec -i aio-postgres psql -U johndoe -d VehicleInventory < scripts/seed-data.sql
--
-- Notlar:
--   • Idempotent: tekrar çalıştırılabilir, var olan kayıtlar atlanır (ON CONFLICT DO NOTHING)
--   • Mevcut Vehicles verileri korunur; sadece VIN çakışması yoksa eklenir
--   • Customers tamamen seed verisidir; e-posta çakışması yoksa eklenir
--   • VehicleOptions için bağımlı kayıtların varlığı DO NOTHING ile güvence altına alınmıştır
-- =============================================================================

-- -----------------------------------------------------------------------------
-- VEHICLES — yeni örnekler (mevcut kayıtlar VIN unique index ile korunur)
-- -----------------------------------------------------------------------------
INSERT INTO "Vehicles"
  ("Id","VIN","Brand","Model","Year","EngineType","Mileage","Color",
   "PurchaseAmount","PurchaseCurrency","SuggestedAmount","SuggestedCurrency",
   "TransmissionType","FuelConsumption","EngineCapacity","Features","Status","CreatedAt")
VALUES
-- Stokta (InStock)
('a1000001-0000-0000-0000-000000000001','JT2BF22K1W0000001','Toyoda',   'Korollo', 2024,'Gasoline',     0,'Beyaz',        750000,   'TRY', 870000,   'TRY','Automatic',  6.5, 1600,'Klima,Isıtmalı Koltuk,Geri Görüş Kamerası',     'InStock', NOW() AT TIME ZONE 'UTC'),
('a1000002-0000-0000-0000-000000000002','WBA3A5C57DF000002','BVM',      '3 Serisi',2023,'Gasoline', 12000,'Siyah',       1200000,  'TRY',1450000,  'TRY','Automatic',  7.2, 2000,'Deri Koltuk,Sunroof,Navigasyon,HUD',             'InStock', NOW() AT TIME ZONE 'UTC'),
('a1000003-0000-0000-0000-000000000003','WAUZZZ8K9DA000003','Awdi',     'A4',      2023,'Diesel',   8500,'Gri',         1050000,  'TRY',1280000,  'TRY','Automatic',  5.8, 2000,'Deri Koltuk,Navigasyon,Adaptif Far',             'InStock', NOW() AT TIME ZONE 'UTC'),
('a1000004-0000-0000-0000-000000000004','5YJSA1DN1DFP00004','Tezla',    'Modell 3',2025,'Electric',     0,'Kırmızı',    1500000,  'TRY',1750000,  'TRY','Automatic',  0.0,    0,'Autopilot,Cam Tavan,Hızlı Şarj',               'InStock', NOW() AT TIME ZONE 'UTC'),
('a1000005-0000-0000-0000-000000000005','VF1LM000000000005','Renolt',   'Meggyn',  2022,'Gasoline', 22000,'Mavi',        480000,   'TRY', 560000,   'TRY','Manual',      6.8, 1300,'Klima,Bluetooth,Geri Sensör',                   'InStock', NOW() AT TIME ZONE 'UTC'),
-- Satışta (OnSale)
('a1000006-0000-0000-0000-000000000006','19XFC1F39NE000006','Howda',    'Civiq',   2023,'Gasoline',  5200,'Gümüş',      680000,   'TRY', 790000,   'TRY','Automatic',  6.2, 1500,'Klima,Geri Görüş Kamerası,Bluetooth',           'OnSale',  NOW() AT TIME ZONE 'UTC'),
('a1000007-0000-0000-0000-000000000007','W0L000000S0000007','Opeel',    'Astro',   2022,'Gasoline', 31000,'Beyaz',      420000,   'TRY', 490000,   'TRY','Manual',      6.4, 1400,'Klima,Bluetooth,Cruise Control',                'OnSale',  NOW() AT TIME ZONE 'UTC'),
('a1000008-0000-0000-0000-000000000008','VSSZZZ6JZ9R000008','Zeat',     'Lione',   2023,'Diesel',    9800,'Kırmızı',    560000,   'TRY', 645000,   'TRY','Automatic',  5.2, 1600,'Deri Koltuk,Navigasyon,Park Asistanı',          'OnSale',  NOW() AT TIME ZONE 'UTC'),
('a1000009-0000-0000-0000-000000000009','WVWZZZ1KZ8W000009','Volxwagen','Golph',   2024,'Hybrid',    1500,'Siyah',      820000,   'TRY', 950000,   'TRY','Automatic',  4.5, 1500,'Klima,Navigasyon,Kablosuz Şarj,Adaptif Hız',   'OnSale',  NOW() AT TIME ZONE 'UTC'),
('a1000010-0000-0000-0000-000000000010','SB1KG58E10F000010','Toyoda',   'ROV4',    2024,'Hybrid',    3200,'Antrasit',   1350000,  'TRY',1550000,  'TRY','Automatic',  5.8, 2500,'Full Ekipman,Deri,Sunroof,360 Kamera',          'OnSale',  NOW() AT TIME ZONE 'UTC'),
-- Rezerve (Reserved) — opsiyon testleri için
('a1000011-0000-0000-0000-000000000011','WMEEJ8AA1FK000011','Mertzedes','KLA',     2023,'Gasoline',  6100,'Beyaz',     1480000,  'TRY',1720000,  'TRY','Automatic',  6.8, 1600,'AMG Paket,Deri,Navigasyon,Isıtmalı Direksiyon', 'Reserved',NOW() AT TIME ZONE 'UTC'),
('a1000012-0000-0000-0000-000000000012','5NPEC4AB0BH000012','Hyowndai', 'Tuksson', 2024,'Hybrid',    2100,'Mavi',       870000,   'TRY', 990000,   'TRY','Automatic',  5.5, 1600,'Panoramik Tavan,Deri,BOSE Ses Sistemi',         'Reserved',NOW() AT TIME ZONE 'UTC'),
-- Satılmış (Sold)
('a1000013-0000-0000-0000-000000000013','1HGCM82633A000013','Howda',    'Akkord',  2021,'Gasoline', 67000,'Gümüş',      480000,   'TRY', 530000,   'TRY','Automatic',  7.4, 2000,'Klima,Bluetooth',                               'Sold',    NOW() AT TIME ZONE 'UTC' - INTERVAL '60 days'),
('a1000014-0000-0000-0000-000000000014','2T1BURHE0JC000014','Toyoda',   'Yariz',   2020,'Gasoline', 98000,'Sarı',       310000,   'TRY', 340000,   'TRY','Manual',      5.9, 1000,'Klima,Bluetooth',                               'Sold',    NOW() AT TIME ZONE 'UTC' - INTERVAL '45 days')
ON CONFLICT ("VIN") DO NOTHING;

-- -----------------------------------------------------------------------------
-- CUSTOMERS — 22 müşteri: 15 bireysel, 7 kurumsal
-- -----------------------------------------------------------------------------
INSERT INTO "Customers"
  ("Id","FirstName","LastName","Email","Phone","CustomerType","CompanyName","TaxNumber","CreatedAt","UpdatedAt")
VALUES
-- Bireysel müşteriler
('b1000001-0000-0000-0000-000000000001','Jon',     'Blaze',     'j.blaze@aio-demo.com',         '+90 532 111 0001','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000002-0000-0000-0000-000000000002','Aise',    'Dogin',     'a.dogin@aio-demo.com',         '+90 542 111 0002','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000003-0000-0000-0000-000000000003','Can Cey', 'Rambo',     'cc.rambo@aio-demo.com',        '+90 505 111 0003','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000004-0000-0000-0000-000000000004','Zyna',    'Plumworth', 'z.plumworth@aio-demo.com',     '+90 551 111 0004','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000005-0000-0000-0000-000000000005','Marv',    'Quickley',  'm.quickley@aio-demo.com',      '+90 531 111 0005','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000006-0000-0000-0000-000000000006','Tiff',    'Wanderby',  't.wanderby@aio-demo.com',      '+90 535 111 0006','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000007-0000-0000-0000-000000000007','Greck',   'Holloway',  'g.holloway@aio-demo.com',      '+90 543 111 0007','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000008-0000-0000-0000-000000000008','Elvy',    'Kurtwood',  'e.kurtwood@aio-demo.com',      '+90 507 111 0008','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000009-0000-0000-0000-000000000009','Huxley',  'Brandyman', 'h.brandyman@aio-demo.com',     '+90 552 111 0009','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000010-0000-0000-0000-000000000010','Mirva',   'Gloonsby',  'm.gloonsby@aio-demo.com',      '+90 530 111 0010','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000011-0000-0000-0000-000000000011','Alf',     'Oztrek',    'a.oztrek@aio-demo.com',        '+90 539 111 0011','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000012-0000-0000-0000-000000000012','Selma',   'Chetnick',  's.chetnick@aio-demo.com',      '+90 545 111 0012','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000013-0000-0000-0000-000000000013','Emriz',   'Bozworth',  'e.bozworth@aio-demo.com',      '+90 533 111 0013','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000014-0000-0000-0000-000000000014','Bushka',  'Platch',    'b.platch@aio-demo.com',        '+90 506 111 0014','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
('b1000015-0000-0000-0000-000000000015','Oko',     'Yildrix',   'o.yildrix@aio-demo.com',       '+90 544 111 0015','Individual',NULL,NULL,NOW() AT TIME ZONE 'UTC',NULL),
-- Kurumsal müşteriler
('b1000016-0000-0000-0000-000000000016','Kemal',   'Acaro',     'kemal.acaro@aio-demo.com',     '+90 212 444 0016','Corporate','Acarlox Logistics Inc.',    '1234567890',NOW() AT TIME ZONE 'UTC',NULL),
('b1000017-0000-0000-0000-000000000017','Deni',    'Erdoganix', 'd.erdoganix@aio-demo.com',     '+90 312 444 0017','Corporate','Bayio Motors Ltd.',         '2345678901',NOW() AT TIME ZONE 'UTC',NULL),
('b1000018-0000-0000-0000-000000000018','Serko',   'Ishwick',   's.ishwick@aio-demo.com',       '+90 216 444 0018','Corporate','Teknoya Invest Corp.',      '3456789012',NOW() AT TIME ZONE 'UTC',NULL),
('b1000019-0000-0000-0000-000000000019','Nilufa',  'Kochsley',  'n.kochsley@aio-demo.com',      '+90 232 444 0019','Corporate','Kochsley Fleet Rental',     '4567890123',NOW() AT TIME ZONE 'UTC',NULL),
('b1000020-0000-0000-0000-000000000020','Tayfun',  'Goolery',   't.goolery@aio-demo.com',       '+90 322 444 0020','Corporate','Goolery Construction Ltd.', '5678901234',NOW() AT TIME ZONE 'UTC',NULL),
('b1000021-0000-0000-0000-000000000021','Cera',    'Ozkano',    'c.ozkano@aio-demo.com',        '+90 242 444 0021','Corporate','Deltamed Healthcare Svcs.',  '6789012345',NOW() AT TIME ZONE 'UTC',NULL),
('b1000022-0000-0000-0000-000000000022','Baris',   'Aslanix',   'b.aslanix@aio-demo.com',       '+90 224 444 0022','Corporate','Aslanix Energy Corp.',      '7890123456',NOW() AT TIME ZONE 'UTC',NULL)
ON CONFLICT ("Email") DO NOTHING;

-- -----------------------------------------------------------------------------
-- VEHICLE OPTIONS — örnek opsiyonlar (Customers ve Vehicles kayıtlarına bağlı)
-- -----------------------------------------------------------------------------
INSERT INTO "VehicleOptions"
  ("Id","VehicleId","CustomerId","ExpiresAt","OptionFeeAmount","OptionFeeCurrency","Notes","Status","CreatedAt","UpdatedAt")
VALUES
-- Aktif opsiyon: Reserved araç 11 ← müşteri Jon Blaze
(
  'c1000001-0000-0000-0000-000000000001',
  'a1000011-0000-0000-0000-000000000011',  -- Mertzedes KLA
  'b1000001-0000-0000-0000-000000000001',  -- Jon Blaze
  NOW() AT TIME ZONE 'UTC' + INTERVAL '7 days',
  5000.00, 'TRY',
  'Müşteri hafta sonu karar verecek',
  'Active',
  NOW() AT TIME ZONE 'UTC',
  NULL
),
-- Aktif opsiyon: Reserved araç 12 ← kurumsal müşteri Kochsley Fleet Rental
(
  'c1000002-0000-0000-0000-000000000002',
  'a1000012-0000-0000-0000-000000000012',  -- Hyowndai Tuksson
  'b1000019-0000-0000-0000-000000000019',  -- Nilufa Kochsley / Kochsley Fleet Rental
  NOW() AT TIME ZONE 'UTC' + INTERVAL '14 days',
  10000.00, 'TRY',
  'Filo alımı değerlendiriliyor, 2 araç daha talep gelebilir',
  'Active',
  NOW() AT TIME ZONE 'UTC',
  NULL
),
-- Süresi dolmuş opsiyon (Expired) — OnSale araç 6 üzerinde geçmiş opsiyon
(
  'c1000003-0000-0000-0000-000000000003',
  'a1000006-0000-0000-0000-000000000006',  -- Howda Civiq
  'b1000003-0000-0000-0000-000000000003',  -- Can Cey Rambo
  NOW() AT TIME ZONE 'UTC' - INTERVAL '3 days',
  2500.00, 'TRY',
  NULL,
  'Expired',
  NOW() AT TIME ZONE 'UTC' - INTERVAL '10 days',
  NOW() AT TIME ZONE 'UTC' - INTERVAL '3 days'
),
-- İptal edilmiş opsiyon (Cancelled) — OnSale araç 9 üzerinde
(
  'c1000004-0000-0000-0000-000000000004',
  'a1000009-0000-0000-0000-000000000009',  -- Volxwagen Golph
  'b1000005-0000-0000-0000-000000000005',  -- Marv Quickley
  NOW() AT TIME ZONE 'UTC' - INTERVAL '1 days',
  0.00, 'TRY',
  'Müşteri fikrini değiştirdi',
  'Cancelled',
  NOW() AT TIME ZONE 'UTC' - INTERVAL '8 days',
  NOW() AT TIME ZONE 'UTC' - INTERVAL '2 days'
)
ON CONFLICT DO NOTHING;

-- -----------------------------------------------------------------------------
-- SERVICE ADVISORS — demo servis danışmanları (şifre: Demo1234!)
-- PBKDF2-SHA256 / 100.000 iterasyon / format: base64(salt):base64(hash)
-- -----------------------------------------------------------------------------
INSERT INTO "ServiceAdvisors" ("Id","FirstName","LastName","Email","PasswordHash","Department","IsActive","CreatedAt")
VALUES
  ('d1000001-0000-0000-0000-000000000001','Wendy','Klorp',  'w.klorp@aio-demo.com', 'FXWTAxIyiJA4arUcOMmnEA==:FFmenqOHyHOfxRJo72ehRZt9cXK77iNIjD5hco9rB5w=','Satış',                   TRUE, NOW() AT TIME ZONE 'UTC'),
  ('d1000002-0000-0000-0000-000000000002','Rex',  'Dunbar', 'r.dunbar@aio-demo.com','Ry1tPuFg7v1tq6EOIDlXTg==:YQJGoPfgUNJWe4dbq3j9yr7+wax3ctYlYok/F7qWcLg=','Teknik Servis',            TRUE, NOW() AT TIME ZONE 'UTC'),
  ('d1000003-0000-0000-0000-000000000003','Jill', 'Sprock', 'j.sprock@aio-demo.com','YWa9gkHbQBg0gYLSDamQTA==:N407AtuFoDdtVpFqWNPmHyuR/IC9N5BSh9JduPEsKhs=','VIP Müşteri Hizmetleri', TRUE, NOW() AT TIME ZONE 'UTC')
ON CONFLICT ("Email") DO NOTHING;

-- -----------------------------------------------------------------------------
-- Özet kontrol sorgusu
-- -----------------------------------------------------------------------------
SELECT 'Vehicles'        AS tablo, COUNT(*) AS kayit FROM "Vehicles"
UNION ALL
SELECT 'Customers'       AS tablo, COUNT(*) AS kayit FROM "Customers"
UNION ALL
SELECT 'VehicleOptions'  AS tablo, COUNT(*) AS kayit FROM "VehicleOptions"
UNION ALL
SELECT 'ServiceAdvisors' AS tablo, COUNT(*) AS kayit FROM "ServiceAdvisors";
