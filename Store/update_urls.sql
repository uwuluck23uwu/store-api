-- Update ShopImageUrl to remove domain
UPDATE dbo.Sellers
SET ShopImageUrl = SUBSTRING(ShopImageUrl, CHARINDEX('/images/', ShopImageUrl), LEN(ShopImageUrl))
WHERE ShopImageUrl LIKE 'http%://%.trycloudflare.com/images/%';

-- Update LogoUrl to remove domain
UPDATE dbo.Sellers
SET LogoUrl = SUBSTRING(LogoUrl, CHARINDEX('/images/', LogoUrl), LEN(LogoUrl))
WHERE LogoUrl LIKE 'http%://%.trycloudflare.com/images/%';

-- Update QrCodeUrl to remove domain
UPDATE dbo.Sellers
SET QrCodeUrl = SUBSTRING(QrCodeUrl, CHARINDEX('/images/', QrCodeUrl), LEN(QrCodeUrl))
WHERE QrCodeUrl LIKE 'http%://%.trycloudflare.com/images/%';

-- Check results
SELECT SellerId, ShopImageUrl, LogoUrl, QrCodeUrl FROM dbo.Sellers;
