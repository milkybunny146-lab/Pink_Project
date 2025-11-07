# ⚡ AWS 部署快速開始

## 🎯 三步驟完成部署

### 第一步：註冊 AWS 帳號（30 分鐘）

1. 前往 https://aws.amazon.com/ 點擊「建立 AWS 帳戶」
2. 準備好：
   - 📧 Email（milkybunny146@gmail.com）
   - 💳 信用卡（用於身份驗證，不會扣款）
   - 📱 手機號碼（接收驗證碼）
3. 按照指示完成註冊
4. 等待 5-10 分鐘帳號啟用

**詳細步驟請參考：[AWS_DEPLOYMENT_GUIDE.md](AWS_DEPLOYMENT_GUIDE.md) 第一部分**

---

### 第二步：安裝工具（15 分鐘）

#### 1. 安裝 AWS CLI

下載並安裝：https://awscli.amazonaws.com/AWSCLIV2.msi

驗證安裝：
```bash
aws --version
```

#### 2. 安裝 EB CLI

```bash
pip install awsebcli --upgrade
```

驗證安裝：
```bash
eb --version
```

#### 3. 設定 AWS 憑證

1. 登入 AWS Console：https://console.aws.amazon.com/
2. 前往：https://console.aws.amazon.com/iam/home#/security_credentials
3. 建立「存取金鑰」，下載 .csv 檔案
4. 執行設定：

```bash
aws configure
```

輸入：
- Access Key ID：（從 .csv 檔案複製）
- Secret Access Key：（從 .csv 檔案複製）
- Region：`ap-southeast-1`
- Output format：`json`

---

### 第三步：部署應用程式（15 分鐘）

#### 方式 A：使用自動化腳本（推薦）

1. 開啟命令提示字元，進入專案資料夾：
   ```bash
   cd C:\Users\User\Documents\GitHub\Pink_Project
   ```

2. 首次部署，初始化：
   ```bash
   eb init
   ```

   回答問題：
   - Region: 選擇 `10) ap-southeast-1`
   - Application name: 按 Enter（使用預設）
   - Platform: 選擇 `.NET Core`
   - CodeCommit: 輸入 `n`
   - SSH: 輸入 `n`

3. 建立環境並部署：
   ```bash
   eb create pinkshop-production
   ```

   ⏱️ 等待 5-10 分鐘...

4. 設定環境變數：
   ```bash
   eb setenv ConnectionStrings__DefaultConnection="Host=ep-holy-mode-a1u70eot-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_Pg6qYtKMT4Of;SSL Mode=VerifyFull;Channel Binding=Require"

   eb setenv EmailSettings__Username="milkybunny146@gmail.com" EmailSettings__Password="sgzlkxfckkmhxcov"
   ```

5. 開啟應用程式：
   ```bash
   eb open
   ```

#### 方式 B：使用部署腳本（後續更新時使用）

已經初始化過後，每次更新只需執行：

```bash
deploy-to-aws.bat
```

---

## ✅ 驗證部署成功

### 1. 檢查應用程式狀態
```bash
eb status
```

應該看到：
- **Health**: Green
- **Status**: Ready

### 2. 測試網站功能

開啟瀏覽器，前往您的應用程式 URL，測試：
- ✅ 首頁是否正常顯示
- ✅ 商品頁面是否載入
- ✅ 購物車是否運作
- ✅ 訂單是否能成功送出
- ✅ 是否收到確認 Email

### 3. 查看日誌（如果有問題）
```bash
eb logs
```

---

## 🔄 後續更新流程

修改程式碼後，重新部署：

```bash
# 方式一：使用腳本
deploy-to-aws.bat

# 方式二：手動執行
git add .
git commit -m "更新功能"
eb deploy
```

---

## 📊 常用指令

```bash
# 查看狀態
eb status

# 查看日誌
eb logs

# 查看最近事件
eb events

# 開啟應用程式
eb open

# 開啟 AWS Console
eb console

# 檢查環境健康狀態
eb health

# 重新啟動應用
eb restart

# 暫停環境（停止收費）
eb terminate pinkshop-production
```

---

## ❗ 重要提醒

### Email 發送限制

AWS EC2 新帳號預設可能限制 Email 發送。如果 Email 無法發送：

1. **選項 A**：申請移除限制
   - 前往：https://aws.amazon.com/premiumsupport/knowledge-center/ec2-port-25-throttle/
   - 提交「移除 Email 發送限制」申請
   - 通常 24-48 小時會批准

2. **選項 B**：改用 AWS SES（Simple Email Service）
   - 前往 AWS SES 控制台
   - 驗證寄件人 Email
   - 使用 SES SMTP 發送（更穩定可靠）

### 費用監控

設定帳單提醒，避免意外費用：

1. 前往 AWS Billing Dashboard
2. 點擊「Budgets」
3. 建立預算提醒（建議設定：$5/月）

---

## 🆘 需要協助？

遇到問題時，請提供：
1. 執行的指令
2. 錯誤訊息
3. `eb logs` 的輸出

我會協助您解決！

---

**準備好了嗎？開始第一步：註冊 AWS 帳號！** 🚀
