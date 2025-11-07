# 🌐 使用 AWS Console 部署（網頁介面）

## 這個方式不需要命令列工具，更適合初學者！

---

## 步驟 1：準備部署檔案

### 1.1 建立部署套件

開啟命令提示字元（CMD），執行：

```bash
cd C:\Users\User\Documents\GitHub\Pink_Project\WebApplication1
dotnet publish -c Release -o publish
```

### 1.2 壓縮發布檔案

將 `C:\Users\User\Documents\GitHub\Pink_Project\WebApplication1\publish` 資料夾中的**所有檔案**（不是資料夾本身）壓縮成 ZIP 檔案。

**重要**：
- 開啟 `publish` 資料夾
- 選取裡面的**所有檔案和資料夾**
- 右鍵 → 傳送到 → 壓縮的資料夾
- 命名為 `pinkshop-deployment.zip`

---

## 步驟 2：登入 AWS Console

1. 前往：https://console.aws.amazon.com/
2. 使用您的 AWS 帳號登入
3. 在上方搜尋欄輸入「Elastic Beanstalk」
4. 點擊進入 Elastic Beanstalk 服務

---

## 步驟 3：建立新應用程式

### 3.1 建立應用程式

1. 點擊 **「建立應用程式」**（Create application）
2. 填寫以下資訊：

   **應用程式名稱（Application name）**
   ```
   pinkshop
   ```

   **應用程式標籤（可選）**
   - 可以不填

3. 點擊下方的 **「設定更多選項」**（Configure more options）

---

### 3.2 選擇平台

在「平台」（Platform）區段：

1. **平台（Platform）**：選擇 **「.NET Core on Linux」**
2. **平台分支（Platform branch）**：選擇最新版本（**.NET 9**）
3. **平台版本（Platform version）**：選擇推薦版本

---

### 3.3 上傳應用程式碼

在「應用程式碼」（Application code）區段：

1. 選擇 **「上傳您的程式碼」**（Upload your code）
2. **版本標籤（Version label）**：輸入 `v1.0`
3. 點擊 **「選擇檔案」**（Choose file）
4. 選擇剛才建立的 `pinkshop-deployment.zip`
5. 等待上傳完成

---

### 3.4 設定預設組態

在「預設組態」（Presets）區段：

- 選擇 **「單一執行個體（免費方案資格）」**（Single instance - Free tier eligible）

這樣可以使用 AWS 免費方案！

---

### 3.5 設定環境變數

1. 找到「軟體」（Software）區段
2. 點擊 **「編輯」**（Edit）
3. 捲動到「環境屬性」（Environment properties）
4. 新增以下環境變數：

   | 名稱 | 值 |
   |------|-----|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |
   | `ConnectionStrings__DefaultConnection` | `Host=ep-holy-mode-a1u70eot-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_Pg6qYtKMT4Of;SSL Mode=VerifyFull;Channel Binding=Require` |
   | `EmailSettings__Username` | `milkybunny146@gmail.com` |
   | `EmailSettings__Password` | `sgzlkxfckkmhxcov` |
   | `EmailSettings__SmtpHost` | `smtp.gmail.com` |
   | `EmailSettings__SmtpPort` | `587` |
   | `EmailSettings__EnableSsl` | `true` |
   | `EmailSettings__SenderEmail` | `milkybunny146@gmail.com` |
   | `EmailSettings__SenderName` | `品渴茶鋪` |
   | `EmailSettings__UseSendGrid` | `false` |

5. 點擊 **「儲存」**（Save）

---

### 3.6 建立環境

1. 檢查所有設定都正確
2. 點擊頁面底部的 **「建立環境」**（Create environment）
3. ⏱️ **等待 5-10 分鐘**，AWS 會自動：
   - 建立 EC2 執行個體
   - 設定安全群組
   - 部署您的應用程式
   - 配置負載平衡器

您會看到進度條，顯示部署狀態。

---

## 步驟 4：驗證部署

### 4.1 檢查環境狀態

部署完成後，您應該會看到：
- **健康狀態（Health）**：綠色的「Ok」
- **環境 URL**：類似 `pinkshop-env.xxxxxx.ap-southeast-1.elasticbeanstalk.com`

### 4.2 開啟應用程式

1. 點擊頁面上方的環境 URL
2. 您的品渴茶鋪網站應該會開啟！

### 4.3 測試功能

- ✅ 瀏覽商品頁面
- ✅ 加入購物車
- ✅ 送出訂單
- ✅ 檢查是否收到 Email

---

## 步驟 5：查看日誌（如果有問題）

如果網站沒有正常運作：

1. 在 Elastic Beanstalk 控制台左側選單
2. 點擊 **「日誌」**（Logs）
3. 點擊 **「請求日誌」** → **「最後 100 行」**（Request Logs → Last 100 Lines）
4. 點擊 **「下載」**（Download）
5. 開啟日誌檔案查看錯誤訊息

---

## 🔄 後續更新應用程式

當您修改程式碼後，要重新部署：

### 方式一：透過 Console

1. 重新執行 `dotnet publish`
2. 建立新的 ZIP 檔案
3. 在 Elastic Beanstalk Console
4. 點擊 **「上傳並部署」**（Upload and deploy）
5. 選擇新的 ZIP 檔案
6. 輸入新的版本標籤（如 `v1.1`）
7. 點擊 **「部署」**（Deploy）

### 方式二：透過 Git（進階）

如果您想自動化，可以設定 AWS CodePipeline 連接您的 GitHub。

---

## 📊 監控和管理

### 查看監控指標

在左側選單：
- **監控（Monitoring）**：查看 CPU、記憶體、請求數等
- **健康狀態（Health）**：查看詳細健康檢查

### 設定警示

1. 點擊 **「警示（Alarms）」**
2. 建立新警示（如：CPU 使用率 > 80%）

### 查看環境變數

1. 點擊左側 **「組態（Configuration）」**
2. 找到 **「軟體（Software）」**
3. 點擊 **「編輯」**查看或修改環境變數

---

## ⚠️ 重要提醒

### Email 發送問題

如果 Email 無法發送，可能是因為 AWS 限制：

**解決方式 A：申請移除限制**
1. 前往 AWS Support Center
2. 建立案例（Create case）
3. 選擇「Service Limit Increase」
4. 選擇「EC2 Email Sending Limits」
5. 說明您要發送訂單確認郵件
6. 通常 24-48 小時會批准

**解決方式 B：使用 AWS SES**
1. 前往 AWS SES 控制台
2. 驗證寄件人 Email
3. 使用 SES SMTP 端點（更可靠）

---

## 💰 費用控制

### 免費方案

AWS 免費方案（12 個月）包含：
- ✅ t2.micro 執行個體：750 小時/月
- ✅ 5GB 儲存空間
- ✅ 15GB 資料傳輸

### 避免超出費用

1. 使用「單一執行個體」組態
2. 在 Billing Dashboard 設定預算警示
3. 不用時可以刪除環境（在 Elastic Beanstalk 點擊「動作」→「終止環境」）

---

## 🎯 完成！

您的品渴茶鋪已經成功部署到 AWS 了！

**您的網站 URL：**
```
https://your-env-name.ap-southeast-1.elasticbeanstalk.com
```

需要協助或遇到問題，隨時告訴我！🚀
