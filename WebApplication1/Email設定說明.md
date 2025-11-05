# 購物車訂購系統 - Email通知功能設定說明

## 📋 功能概述

本次更新為購物車訂購系統添加了以下功能：

### ✅ 新增欄位
1. **Email欄位** - 顧客可填寫電子郵件地址
2. **取貨方式選擇** - 可選擇「自取」或「外送」
3. **訂單確認Email** - 訂單成功後自動發送確認信至顧客信箱

### 📊 資料庫變更
在 `Pinkshop_Orders` 資料表中添加了兩個新欄位：
- `CustomerEmail` (NVARCHAR(255)) - 儲存顧客Email
- `DeliveryType` (NVARCHAR(50)) - 儲存取貨方式（自取/外送）

---

## 🔧 設定步驟

### 步驟 1：更新資料庫結構

執行資料庫更新腳本：

```sql
-- 在 SQL Server Management Studio 中執行
-- 檔案位置: WebApplication1/database_update.sql
```

或者手動執行以下SQL：

```sql
USE mydb;

-- 添加 Email 欄位
ALTER TABLE Pinkshop_Orders
ADD CustomerEmail NVARCHAR(255) NULL;

-- 添加取貨方式欄位
ALTER TABLE Pinkshop_Orders
ADD DeliveryType NVARCHAR(50) NULL;
```

### 步驟 2：設定SMTP郵件伺服器

在 `HomeController.cs` 的 `SendOrderConfirmationEmail` 方法中，找到以下程式碼並修改為您的SMTP設定：

```csharp
// 目前位置：第440-444行
SmtpClient smtp = new SmtpClient();
smtp.Host = "smtp.gmail.com";        // 修改為您的SMTP伺服器
smtp.Port = 587;                     // 修改為您的SMTP埠號
smtp.EnableSsl = true;
smtp.Credentials = new NetworkCredential(
    "your-email@gmail.com",          // 修改為您的Email帳號
    "your-app-password"              // 修改為您的Email密碼或應用程式密碼
);
```

### 步驟 3：選擇Email服務提供商

#### 選項 A：使用 Gmail

1. **啟用兩步驟驗證**
   - 前往 Google 帳戶設定
   - 啟用兩步驟驗證

2. **建立應用程式密碼**
   - 前往 https://myaccount.google.com/apppasswords
   - 選擇「郵件」和「Windows電腦」
   - 生成應用程式密碼

3. **配置設定**
```csharp
smtp.Host = "smtp.gmail.com";
smtp.Port = 587;
smtp.EnableSsl = true;
smtp.Credentials = new NetworkCredential(
    "your-email@gmail.com",
    "your-16-digit-app-password"    // 使用生成的應用程式密碼
);
```

#### 選項 B：使用 Outlook/Office 365

```csharp
smtp.Host = "smtp.office365.com";
smtp.Port = 587;
smtp.EnableSsl = true;
smtp.Credentials = new NetworkCredential(
    "your-email@outlook.com",
    "your-password"
);
```

#### 選項 C：使用其他SMTP服務

- SendGrid
- Mailgun
- Amazon SES
- 自架SMTP伺服器

請參考各服務提供商的文件進行設定。

---

## 🎨 功能特色

### 1. 購物車頁面 (Cart.cshtml)

**新增欄位：**
- ✉️ Email欄位（必填）- 自動驗證Email格式
- 🚚 取貨方式（必填）- 下拉選單「自取」或「外送」
- 📍 外送地址（條件必填）- 選擇外送時才顯示並要求填寫

**智慧表單驗證：**
- Email格式驗證
- 電話號碼格式驗證（10位數字）
- 外送地址條件驗證

### 2. 訂單確認頁面 (OrderConfirmation.cshtml)

**顯示內容：**
- 顧客Email地址
- 取貨方式（自取顯示綠色，外送顯示粉紅色）
- 外送地址（僅當選擇外送時顯示）
- Email發送確認提示

**智慧提醒：**
- 根據取貨方式顯示不同的提醒訊息
- 自取：提醒顧客到店取餐
- 外送：提醒會安排配送

### 3. Email通知功能

**Email內容包含：**
- 訂單編號
- 顧客姓名
- 商品詳情（名稱、尺寸、數量）
- 取貨方式
- 外送地址（如適用）
- 訂單總金額
- 溫馨提醒事項

**Email樣式：**
- 專業的HTML格式
- 品牌色彩（粉紅色主題）
- 響應式設計
- 清晰的排版和圖示

---

## 🧪 測試建議

### 測試流程：

1. **基本訂單測試（自取）**
   - 選擇商品 → 填寫資料
   - 選擇「自取」
   - 確認不需要填寫地址
   - 送出訂單

2. **外送訂單測試**
   - 選擇商品 → 填寫資料
   - 選擇「外送」
   - 確認地址欄位出現並為必填
   - 填寫地址後送出訂單

3. **Email發送測試**
   - 使用真實的Email地址
   - 確認Email是否成功發送
   - 檢查Email內容是否正確
   - 確認Email格式是否美觀

4. **錯誤處理測試**
   - 測試Email格式驗證
   - 測試必填欄位驗證
   - 測試SMTP連線錯誤處理

---

## ⚠️ 注意事項

### 安全性建議：

1. **不要在程式碼中直接寫入敏感資訊**
   - SMTP密碼應該儲存在 `appsettings.json` 或環境變數中
   - 考慮使用 Azure Key Vault 或其他密鑰管理服務

2. **建議的配置方式（appsettings.json）：**

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@pinkshop.com",
    "SenderName": "品渴茶鋪",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

3. **讀取配置的程式碼範例：**

```csharp
// 在 Controller 中注入 IConfiguration
private readonly IConfiguration _configuration;

public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
{
    _logger = logger;
    _configuration = configuration;
}

// 在 SendOrderConfirmationEmail 方法中使用
var smtpHost = _configuration["EmailSettings:SmtpHost"];
var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
var username = _configuration["EmailSettings:Username"];
var password = _configuration["EmailSettings:Password"];
```

### 效能考量：

1. **非同步發送**
   - 目前Email是同步發送的
   - 建議改為非同步以提升效能
   - 可考慮使用背景工作或訊息佇列

2. **錯誤處理**
   - 目前Email發送失敗不會影響訂單建立
   - 建議記錄失敗的Email發送，稍後重試

---

## 📝 資料庫欄位說明

### Pinkshop_Orders 表

| 欄位名稱 | 資料型別 | 可為NULL | 說明 |
|---------|----------|---------|------|
| CustomerEmail | NVARCHAR(255) | YES | 顧客電子郵件地址 |
| DeliveryType | NVARCHAR(50) | YES | 取貨方式（自取/外送） |

---

## 🐛 常見問題排解

### 問題 1：Email無法發送

**可能原因：**
- SMTP設定錯誤
- 防火牆封鎖
- Email帳號安全設定

**解決方案：**
1. 檢查SMTP設定是否正確
2. 確認網路連線
3. 檢查防火牆設定
4. 確認Email帳號已啟用「低安全性應用程式存取」或使用應用程式密碼

### 問題 2：資料庫欄位錯誤

**錯誤訊息：** "Invalid column name 'CustomerEmail'"

**解決方案：**
1. 確認已執行資料庫更新腳本
2. 重新整理資料庫連線
3. 檢查欄位名稱是否正確

### 問題 3：表單驗證失敗

**可能原因：**
- JavaScript未正確載入
- 瀏覽器相容性問題

**解決方案：**
1. 清除瀏覽器快取
2. 檢查瀏覽器控制台錯誤訊息
3. 確認JavaScript程式碼正確載入

---

## 📞 技術支援

如有任何問題或需要協助，請：
1. 檢查本文件的「常見問題排解」章節
2. 查看控制台的錯誤訊息
3. 檢查資料庫連線狀態
4. 確認SMTP設定正確

---

## 📅 更新記錄

- **2025-10-28**：初版發布
  - 添加Email欄位
  - 添加取貨方式選擇
  - 實作Email通知功能
  - 建立資料庫更新腳本

---

祝使用愉快！🎉

