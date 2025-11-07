# 🚀 AWS Elastic Beanstalk 部署完整指南

## 📋 前置準備

### 您需要準備：
1. ✅ 信用卡（AWS 帳號註冊需要，但不會收費）
2. ✅ 電話號碼（接收驗證碼）
3. ✅ Email 帳號
4. ⏱️ 約 60-90 分鐘的時間

---

## 第一部分：註冊 AWS 帳號

### 步驟 1：前往 AWS 官網註冊

1. 開啟瀏覽器，前往：https://aws.amazon.com/
2. 點擊右上角的 **「建立 AWS 帳戶」** 或 **「Create an AWS Account」**

### 步驟 2：填寫基本資料

1. **Email 地址**：輸入您的 Email（建議用 milkybunny146@gmail.com）
2. **AWS 帳戶名稱**：輸入 `PinkShop` 或任何您喜歡的名稱
3. 點擊 **「驗證電子郵件地址」**
4. 到您的信箱收取驗證碼並輸入

### 步驟 3：設定密碼

1. 設定一個強密碼（至少 8 個字元，包含大小寫字母和數字）
2. 確認密碼
3. 點擊 **「繼續」**

### 步驟 4：填寫聯絡資訊

1. **帳戶類型**：選擇 **「個人」**（Personal）
2. **全名**：填寫您的真實姓名
3. **電話號碼**：填寫台灣手機號碼（格式：+886912345678）
4. **國家/地區**：選擇 **「台灣」**
5. **地址**：填寫完整地址
6. 勾選「我已閱讀並同意 AWS 客戶協議」
7. 點擊 **「繼續」**

### 步驟 5：輸入付款資訊

⚠️ **重要說明**：
- AWS 需要信用卡驗證身份，但**不會立即收費**
- 您可以使用**免費方案** 12 個月
- 只有超過免費額度才會收費
- 會先扣 $1 美元驗證，之後會退回

1. **付款方式**：輸入信用卡資訊
   - 卡號
   - 到期日期
   - CVV（卡片背面的 3 碼）
2. **帳單地址**：填寫信用卡帳單地址
3. 點擊 **「驗證並繼續」**

### 步驟 6：驗證電話號碼

1. 選擇驗證方式：**「簡訊」**或**「語音通話」**
2. 輸入手機號碼
3. 點擊 **「發送驗證碼」**
4. 輸入收到的 4 位數驗證碼
5. 點擊 **「繼續」**

### 步驟 7：選擇支援方案

1. 選擇 **「基本支援 - 免費」**（Basic Support - Free）
2. 點擊 **「完成註冊」**

### 步驟 8：等待帳號啟用

1. 您會看到「歡迎使用 AWS」的頁面
2. 通常需要等待 **5-10 分鐘**帳號才會完全啟用
3. 您會收到一封確認 Email

---

## 第二部分：安裝必要工具

### 工具 1：安裝 AWS CLI

#### Windows 安裝方式：

1. 下載 AWS CLI 安裝程式：
   - 前往：https://aws.amazon.com/cli/
   - 點擊 **「Download for Windows」**
   - 或直接下載：https://awscli.amazonaws.com/AWSCLIV2.msi

2. 執行下載的 `.msi` 檔案
3. 按照安裝精靈指示完成安裝
4. 安裝完成後，開啟 **命令提示字元 (CMD)** 或 **PowerShell**
5. 驗證安裝：
   ```bash
   aws --version
   ```
   應該會顯示類似：`aws-cli/2.x.x Python/3.x.x Windows/...`

### 工具 2：安裝 EB CLI（Elastic Beanstalk CLI）

1. 首先確認已安裝 Python（AWS CLI 會自動安裝）
2. 開啟命令提示字元，執行：
   ```bash
   pip install awsebcli --upgrade
   ```
3. 驗證安裝：
   ```bash
   eb --version
   ```
   應該會顯示類似：`EB CLI 3.x.x`

---

## 第三部分：設定 AWS 憑證

### 步驟 1：建立存取金鑰 (Access Key)

1. 登入 AWS Console：https://console.aws.amazon.com/
2. 點擊右上角的帳戶名稱，選擇 **「安全憑證」**（Security Credentials）
3. 或直接前往：https://console.aws.amazon.com/iam/home#/security_credentials
4. 展開 **「存取金鑰 (存取金鑰 ID 和秘密存取金鑰)」**
5. 點擊 **「建立新的存取金鑰」**
6. ⚠️ **重要**：立即下載 `.csv` 檔案並妥善保存
   - Access Key ID（類似：`AKIAIOSFODNN7EXAMPLE`）
   - Secret Access Key（類似：`wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY`）
   - **這是唯一一次可以看到 Secret Key 的機會！**

### 步驟 2：設定 AWS CLI

開啟命令提示字元，執行：

```bash
aws configure
```

依序輸入：
1. **AWS Access Key ID**：貼上您剛才的 Access Key ID
2. **AWS Secret Access Key**：貼上您的 Secret Access Key
3. **Default region name**：輸入 `ap-southeast-1`（新加坡，離台灣最近）
4. **Default output format**：輸入 `json`

---

## 第四部分：部署到 Elastic Beanstalk

### 步驟 1：初始化 Elastic Beanstalk 應用

1. 開啟命令提示字元，切換到專案資料夾：
   ```bash
   cd C:\Users\User\Documents\GitHub\Pink_Project
   ```

2. 初始化 EB 應用：
   ```bash
   eb init
   ```

3. 回答以下問題：
   - **Select a default region**：選擇 `10) ap-southeast-1 : Asia Pacific (Singapore)`
   - **Enter Application Name**：輸入 `pinkshop` 或按 Enter 使用預設
   - **It appears you are using .NET Core. Is this correct?**：輸入 `y`
   - **Select a platform branch**：選擇最新的 `.NET Core on Linux`
   - **Do you wish to continue with CodeCommit?**：輸入 `n`
   - **Do you want to set up SSH?**：輸入 `n`（新手建議選 n）

### 步驟 2：建立 Elastic Beanstalk 環境

```bash
eb create pinkshop-production
```

這個指令會：
- 建立 EC2 執行個體
- 設定負載平衡器
- 配置安全群組
- 部署您的應用程式

⏱️ **預計需要 5-10 分鐘**，請耐心等待。

### 步驟 3：設定環境變數（資料庫連線字串）

部署完成後，設定資料庫連線：

```bash
eb setenv ConnectionStrings__DefaultConnection="Host=ep-holy-mode-a1u70eot-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_Pg6qYtKMT4Of;SSL Mode=VerifyFull;Channel Binding=Require"
```

設定 Email 相關環境變數：

```bash
eb setenv EmailSettings__Username="milkybunny146@gmail.com" EmailSettings__Password="sgzlkxfckkmhxcov"
```

### 步驟 4：開啟應用程式

```bash
eb open
```

這會自動開啟瀏覽器，顯示您的應用程式！

---

## 第五部分：後續更新部署

當您修改程式碼後，要重新部署：

1. 提交程式碼到 Git：
   ```bash
   git add .
   git commit -m "更新訂單功能"
   ```

2. 部署到 AWS：
   ```bash
   eb deploy
   ```

3. 等待 2-3 分鐘完成部署

---

## 📊 監控和管理

### 查看應用程式狀態
```bash
eb status
```

### 查看日誌（Log）
```bash
eb logs
```

### 查看最近的事件
```bash
eb events
```

### 開啟 AWS Console 管理介面
```bash
eb console
```

---

## 💰 費用說明

### 免費方案（12 個月）包含：
- ✅ EC2 t2.micro 執行個體：750 小時/月
- ✅ 負載平衡器（部分免費）
- ✅ 5GB 標準儲存空間
- ✅ 15GB 資料傳輸

### 小流量網站（茶飲店）預估費用：
- **前 12 個月**：幾乎免費（只要不超過額度）
- **12 個月後**：約 $5-15 美元/月（依流量而定）

### ⚠️ 如何避免意外收費：
1. 定期檢查 AWS Billing Dashboard
2. 設定帳單提醒（Billing Alerts）
3. 不用時可以暫停環境：`eb terminate pinkshop-production`

---

## 🛠️ 疑難排解

### 問題 1：部署失敗
```bash
# 查看詳細錯誤訊息
eb logs
```

### 問題 2：應用程式無法啟動
```bash
# 檢查環境狀態
eb health
```

### 問題 3：Email 無法發送
- 檢查環境變數是否正確設定
- AWS EC2 預設可能限制 port 25，但 port 587 可以使用
- 如果還是不行，可能需要申請移除 Email 發送限制：
  https://aws.amazon.com/premiumsupport/knowledge-center/ec2-port-25-throttle/

---

## 🎯 下一步

部署成功後，您可以：

1. ✅ 測試訂單功能
2. ✅ 測試 Email 發送
3. ✅ 設定自訂網域名稱
4. ✅ 啟用 HTTPS（Elastic Beanstalk 可自動設定）
5. ✅ 設定自動擴展（Auto Scaling）

---

## 📞 需要協助？

如果遇到任何問題，請告訴我：
1. 您在哪個步驟遇到問題
2. 錯誤訊息是什麼
3. 執行 `eb logs` 的輸出

我會協助您解決！

---

**祝部署順利！🎉**
