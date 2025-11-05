# 🍵 品渴茶鋪 - 線上訂購系統

> 一個功能完整的茶飲店線上訂購與管理系統

![](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)
![](https://img.shields.io/badge/C%23-Latest-green)
![](https://img.shields.io/badge/SQL_Server-2019+-red)
![](https://img.shields.io/badge/HTML5-CSS3-orange)
![](https://img.shields.io/badge/JavaScript-ES6+-yellow)

---

## 📋 專案簡介

品渴茶鋪是一個功能完整的飲料店線上訂購系統，提供直觀的使用者介面、完整的購物車功能、智慧表單驗證，以及自動化的Email訂單確認通知。

### 🎯 專案目標

- 提供便捷的線上訂購體驗
- 實現完整的購物車功能
- 自動化訂單處理與通知
- 靈活的取貨方式選擇

---

## ✨ 核心功能

### 1. 商品展示系統
- **動態商品分類**：從資料庫動態載入商品類別
- **商品詳細資訊**：名稱、價格、描述、圖片
- **多尺寸選擇**：支援中杯、大杯等不同尺寸
- **即時價格計算**：根據尺寸和數量自動計算

### 2. 智慧購物車系統
- **LocalStorage持久化**：購物車資料保存在本地，刷新不遺失
- **多商品累積**：支援添加多個不同商品
- **自動合併**：相同商品（名稱+尺寸）自動合併數量
- **即時更新**：動態顯示購物車商品數量和總金額

### 3. 完整填單表單
- **必填欄位驗證**：
  - 姓名
  - 電話（10位數字格式驗證）
  - Email（格式驗證）
  - 取貨方式（自取/外送）
  - 付款方式
  
- **條件驗證**：
  - 選擇外送時，地址為必填
  - 選擇自取時，地址不需填寫
  
- **即時回饋**：
  - 格式錯誤即時提示
  - 成功訊息明確顯示

### 4. 訂單處理系統
- **自動訂單編號生成**：格式 `ORD + 日期 + 流水號`
- **交易完整性**：使用SQL Transaction確保資料一致性
- **多商品訂單支援**：單筆訂單可包含多個商品
- **訂單狀態管理**：待處理、已完成等狀態

### 5. Email通知功能
- **自動發送**：訂單成功後自動發送確認信
- **美觀模板**：專業的HTML格式
- **完整資訊**：
  - 訂單編號
  - 商品明細（名稱、尺寸、數量、價格）
  - 取貨方式
  - 外送地址（如適用）
  - 訂單總金額
  
- **SMTP整合**：支援Gmail、Outlook等多種郵件服務

### 6. 訂單確認頁面
- **成功動畫**：視覺化的成功提示
- **完整資訊展示**：顧客資訊、商品明細、總金額
- **清晰的後續指引**：根據取貨方式顯示不同提醒

---

## 🛠 技術架構

### 後端技術棧
- **框架**：ASP.NET Core 8.0 MVC
- **語言**：C# (Latest)
- **資料庫**：SQL Server 2019+
- **ORM**：ADO.NET (SqlConnection, SqlCommand)
- **郵件服務**：System.Net.Mail (SMTP)

### 前端技術棧
- **視圖引擎**：Razor Pages
- **樣式**：CSS3, Bootstrap 5
- **腳本**：原生 JavaScript (ES6+)
- **儲存**：LocalStorage API
- **UI/UX**：響應式設計 (RWD)

### 資料庫設計
- **Pinkshop_Categories**：商品分類表
- **Pinkshop_Products**：商品資料表
- **Pinkshop_Prices**：商品價格表
- **Pinkshop_Orders**：訂單主表
- **Pinkshop_OrderDetails**：訂單明細表

---

## 💾 資料庫結構

### Pinkshop_Orders（訂單主表）
```sql
- Id (INT, PRIMARY KEY, IDENTITY)
- OrderNumber (NVARCHAR(50), UNIQUE)
- CustomerName (NVARCHAR(100))
- CustomerPhone (NVARCHAR(20))
- CustomerEmail (NVARCHAR(255))
- DeliveryType (NVARCHAR(50))
- CustomerAddress (NVARCHAR(500))
- TotalAmount (DECIMAL(10, 2))
- OrderStatus (NVARCHAR(50))
- PaymentStatus (NVARCHAR(50))
- PaymentMethod (NVARCHAR(50))
- CreatedAt (DATETIME)
```

### Pinkshop_OrderDetails（訂單明細表）
```sql
- Id (INT, PRIMARY KEY, IDENTITY)
- OrderId (INT, FOREIGN KEY)
- ProductId (INT)
- ProductName (NVARCHAR(200))
- SizeName (NVARCHAR(50))
- Quantity (INT)
- UnitPrice (DECIMAL(10, 2))
- Subtotal (DECIMAL(10, 2))
- CreatedAt (DATETIME)
```

---

## 🎨 特色功能

### 1. 購物車體驗優化
- **兩種購買模式**：
  - 「加入購物車」：累積多個商品後統一結帳
  - 「立即購買」：直接進入結帳流程
  
- **友善的使用者提示**：
  - 加入購物車後顯示當前購物車商品數量
  - 提示用戶下一步操作（繼續選購或結帳）

### 2. 智慧表單設計
- **動態欄位顯示**：
  - 取貨方式為「外送」時才顯示地址欄位
  - 減少不必要的欄位，提升填寫體驗
  
- **多重驗證機制**：
  - 前端JavaScript驗證
  - HTML5原生驗證
  - 後端資料驗證

### 3. 視覺化訂單摘要
- **表格式呈現**：購物車商品以清晰的表格顯示
- **即時更新**：選擇商品時即時顯示總金額
- **色彩區分**：使用品牌色強調重要資訊

### 4. Email模板設計
- **品牌風格**：粉紅色主題，符合品牌形象
- **響應式設計**：在手機和電腦上都能正常顯示
- **清晰的資訊架構**：訂單資訊一目了然

---

## 📱 響應式設計

### 支援裝置
- 桌上型電腦（1920px+）
- 平板電腦（768px - 1024px）
- 手機（375px - 768px）

### 適配功能
- 彈性佈局 (Flexbox)
- 媒體查詢 (Media Queries)
- 觸控友善的按鈕尺寸
- 移動優先設計原則

---

## 🔒 安全性設計

### 資料驗證
- 前端表單驗證
- 後端參數驗證
- SQL注入防護（使用參數化查詢）

### 密碼管理
- Email密碼儲存在配置檔案
- 建議使用環境變數或Key Vault
- 不將敏感資訊提交到版本控制

### 錯誤處理
- Try-Catch異常捕獲
- 交易回滾機制
- 友善的錯誤提示

---

## 🚀 部署建議

### 本地開發環境
1. Visual Studio 2022
2. .NET 8.0 SDK
3. SQL Server 2019+ / SQL Server Express
4. IIS Express (內建)

### 生產環境選項

#### 選項 1：Azure App Service
- 快速部署
- 自動擴展
- 內建SSL憑證

#### 選項 2：IIS
- Windows Server
- 完整控制權
- 適合企業內部

#### 選項 3：Docker
- 容器化部署
- 跨平台支援
- 易於維護

---

## 📊 效能優化

### 已實施
- **資料庫索引**：訂單編號、時間、Email
- **圖片優化**：適當的圖片尺寸
- **LocalStorage**：減少伺服器請求
- **非同步處理**：Email發送不阻塞主流程

### 可改進方向
- 引入快取機制（Redis）
- 前端資源壓縮與合併
- CDN靜態資源托管
- 資料庫查詢優化

---

## 🧪 測試建議

### 單元測試
- Controller方法測試
- 資料驗證邏輯測試
- Email發送功能測試

### 整合測試
- 完整訂單流程測試
- 資料庫CRUD測試
- API端點測試

### UI測試
- 瀏覽器相容性測試
- 響應式佈局測試
- 使用者體驗測試

---

## 📈 未來擴展方向

### 短期改進
1. **會員系統**
   - 註冊/登入功能
   - 訂單歷史查詢
   - 收藏商品

2. **後台管理**
   - 訂單管理介面
   - 商品管理
   - 報表統計

3. **支付整合**
   - 線上支付（綠界、藍新）
   - 電子發票
   - 訂單追蹤

### 長期規劃
1. **APP開發**
   - iOS / Android應用
   - 推播通知
   - 行動支付

2. **進階功能**
   - AI推薦系統
   - 會員點數
   - 優惠券系統
   - 多店鋪支援

3. **數據分析**
   - 銷售報表
   - 顧客分析
   - 庫存預測

---

## 💡 開發亮點

### 1. 完整的購物車邏輯
- 不僅是簡單的「加入購物車」功能
- 實現了真實電商的購物車體驗
- 支援多商品、自動合併、持久化

### 2. 用戶體驗設計
- 清晰的操作流程
- 即時的視覺反饋
- 友善的錯誤提示
- 符合使用者習慣的設計

### 3. 資料完整性
- 使用Transaction確保資料一致性
- 訂單與明細的關聯正確
- 完整的錯誤處理機制

### 4. 郵件整合
- 不僅發送Email，還有精美的HTML模板
- 根據訂單內容動態生成Email內容
- 支援多種SMTP服務

### 5. 程式碼品質
- 清晰的命名規範
- 適當的註解說明
- 良好的程式結構
- 易於維護和擴展

---

## 📞 聯絡資訊

- **專案作者**：[您的名字]
- **Email**：[您的Email]
- **GitHub**：[您的GitHub]
- **LinkedIn**：[您的LinkedIn]

---

## 📝 授權

本專案僅供學習和作品集展示使用。

---

## 🙏 致謝

感謝在開發過程中提供協助和建議的所有人。

---

**最後更新**：2025年10月

**版本**：1.0.0

