-- ========================================
-- 資料庫更新腳本 - 添加Email和取貨方式欄位
-- 執行日期：請在實際執行時記錄
-- ========================================

USE mydb;
GO

-- 檢查並添加 CustomerEmail 欄位
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'Pinkshop_Orders') 
    AND name = 'CustomerEmail'
)
BEGIN
    ALTER TABLE Pinkshop_Orders
    ADD CustomerEmail NVARCHAR(255) NULL;
    
    PRINT '✅ 已成功添加 CustomerEmail 欄位';
END
ELSE
BEGIN
    PRINT 'ℹ️ CustomerEmail 欄位已存在，跳過';
END
GO

-- 檢查並添加 DeliveryType 欄位
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'Pinkshop_Orders') 
    AND name = 'DeliveryType'
)
BEGIN
    ALTER TABLE Pinkshop_Orders
    ADD DeliveryType NVARCHAR(50) NULL;
    
    PRINT '✅ 已成功添加 DeliveryType 欄位';
END
ELSE
BEGIN
    PRINT 'ℹ️ DeliveryType 欄位已存在，跳過';
END
GO

-- 更新現有資料的預設值（可選）
-- 如果您想為現有訂單設定預設的取貨方式
UPDATE Pinkshop_Orders
SET DeliveryType = N'自取'
WHERE DeliveryType IS NULL;
GO

-- 驗證更新
SELECT TOP 5 
    OrderNumber, 
    CustomerName, 
    CustomerEmail, 
    DeliveryType, 
    CustomerAddress,
    CreatedAt
FROM Pinkshop_Orders
ORDER BY CreatedAt DESC;
GO

PRINT '========================================';
PRINT '✅ 資料庫更新完成！';
PRINT '========================================';
GO

