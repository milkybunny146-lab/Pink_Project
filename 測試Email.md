# 📧 Email測試檢查清單

## 當前設定
- **Gmail帳號**: milkybunny146@gmail.com
- **應用程式密碼**: gunxbugjkrtgsyjv
- **SMTP伺服器**: smtp.gmail.com:587

---

## ✅ 檢查清單

### 1. Gmail帳號檢查
- [ ] 前往 https://myaccount.google.com/security
- [ ] 確認「兩步驟驗證」已啟用（必須）
- [ ] 前往 https://myaccount.google.com/apppasswords
- [ ] 確認有顯示「郵件」的應用程式密碼
- [ ] 如果不確定，刪除舊密碼並重新生成

### 2. 應用程式檢查
- [ ] Visual Studio 已按 Shift+F5 停止
- [ ] 等待 3 秒
- [ ] 按 F5 重新啟動
- [ ] 網站成功啟動

### 3. 訂單測試
- [ ] 進入網站
- [ ] 選擇商品並加入購物車
- [ ] 填寫訂單表單
- [ ] **Email欄位填寫測試用Email**（可以是任何有效Email）
- [ ] 提交訂單

### 4. 檢查控制台輸出
在 Visual Studio 下方「輸出」視窗中查看：

**成功的訊息：**
```
✅ 訂單確認Email已成功發送至：test@email.com
```

**失敗的訊息（常見）：**

❌ **"Authentication failed"**
→ 應用程式密碼錯誤，請重新生成

❌ **"Email設定尚未配置"**
→ appsettings.json 未正確設定

❌ **"Could not connect to the remote server"**
→ 網路連線問題或防火牆封鎖

❌ **"Mailbox unavailable"**
→ 收件者Email地址無效

❌ **"5.7.0 Authentication Required"**
→ 需要啟用「低安全性應用程式存取」或使用應用程式密碼

### 5. 檢查Email信箱
- [ ] 檢查收件匣
- [ ] **檢查垃圾郵件夾** ⭐ 最重要！
- [ ] 檢查所有郵件標籤

---

## 🔧 常見問題解決

### 問題 1：Authentication failed（最常見）

**原因：** 應用程式密碼可能不正確或已過期

**解決方案：**
1. 前往 https://myaccount.google.com/apppasswords
2. 刪除現有的「郵件」應用程式密碼
3. 重新生成新的（選擇「郵件」+「Windows電腦」）
4. 複製新密碼（16位數，移除空格）
5. 更新 appsettings.json 中的 Password
6. 重新啟動應用程式

### 問題 2：兩步驟驗證未啟用

如果無法生成應用程式密碼，表示兩步驟驗證未啟用：

1. 前往 https://myaccount.google.com/security
2. 找到「兩步驟驗證」
3. 點擊「開始使用」
4. 完成設定（需要手機驗證）
5. 啟用後才能生成應用程式密碼

### 問題 3：防火牆封鎖

檢查防火牆是否允許：
- Port 587 (SMTP with TLS)
- Port 465 (SMTP with SSL)

### 問題 4：Gmail安全性設定

確認Gmail沒有封鎖來自應用程式的登入：
1. 前往 https://myaccount.google.com/notifications
2. 檢查是否有「已封鎖的登入嘗試」
3. 如果有，點擊「是我本人」

---

## 🧪 手動測試步驟

### 快速測試（5分鐘）

1. **停止並重新啟動應用程式**
   ```
   Visual Studio: Shift+F5 → 等3秒 → F5
   ```

2. **進入網站並下測試訂單**
   - 選擇任一商品
   - 加入購物車
   - 點擊「立即購買」
   - 填寫表單：
     * 姓名：測試
     * 電話：0912345678
     * **Email：您自己的Email（可以用 milkybunny146@gmail.com）**
     * 取貨方式：自取
     * 付款方式：現金
   - 提交訂單

3. **立即檢查 Visual Studio 輸出視窗**
   - 看是否有「✅ Email已成功發送」
   - 或是有什麼錯誤訊息

4. **檢查Email信箱**
   - 包括垃圾郵件夾！

---

## 💡 如果還是不行

請提供以下資訊：

1. **Visual Studio 輸出視窗中的錯誤訊息**（完整的）
2. **兩步驟驗證是否已啟用**
3. **應用程式密碼頁面是否有顯示「郵件」項目**
4. **是否檢查過垃圾郵件夾**

---

## 📝 臨時替代方案

如果Gmail一直有問題，可以暫時改用其他服務：

### 選項 A：使用 Outlook

如果您有 Outlook 帳號，可以改用：

```json
"EmailSettings": {
  "SmtpHost": "smtp.office365.com",
  "SmtpPort": 587,
  "EnableSsl": true,
  "SenderEmail": "您的Outlook@outlook.com",
  "Username": "您的Outlook@outlook.com",
  "Password": "您的Outlook密碼"
}
```

### 選項 B：暫時停用Email功能

如果只是測試，可以暫時註解Email發送：

在 `HomeController.cs` 中找到：
```csharp
// 發送訂單確認Email
try
{
    SendOrderConfirmationEmail(...);
}
```

改成：
```csharp
// 暫時停用Email發送
// try
// {
//     SendOrderConfirmationEmail(...);
// }
```

---

**下一步：請告訴我 Visual Studio 輸出視窗中顯示什麼訊息！** 🔍

