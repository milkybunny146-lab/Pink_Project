using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    // 購物車商品類別
    public class CartItem
    {
        public string name { get; set; } = string.Empty;
        public string size { get; set; } = string.Empty;
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
        public string image { get; set; } = string.Empty;
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PinkshopDbContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, PinkshopDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("開始查詢分類資料...");

                // 使用 EF Core LINQ 查詢分類
                var categoriesData = await _context.Categories
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.DisplayOrder)
                    .ToListAsync();

                // 轉換為字典格式
                var categories = categoriesData.Select(c => new Dictionary<string, object>
                {
                    ["Id"] = c.Id,
                    ["Name"] = c.Name,
                    ["DisplayOrder"] = c.DisplayOrder,
                    ["IsActive"] = c.IsActive,
                    ["CreatedAt"] = c.CreatedAt.ToString()
                }).ToList();

                _logger.LogInformation("查詢完成，找到 {Count} 個分類", categories.Count);
                ViewData["categories"] = categories;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢分類時發生錯誤");
                ViewData["categories"] = new List<Dictionary<string, object>>();
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public async Task<IActionResult> Cart(string item, string medium, string large, string image)
        {
            // 從URL參數獲取商品資訊
            ViewData["ItemName"] = item ?? "圍爐奶茶";
            ViewData["MediumPrice"] = medium ?? "70";
            ViewData["LargePrice"] = large ?? "90";
            ViewData["ImageName"] = image ?? "orginal.jpg";

            // 如果有商品名稱，從資料庫獲取詳細資訊
            if (!string.IsNullOrEmpty(item))
            {
                var productInfo = await GetProductFromDatabase(item);
                ViewData["ProductInfo"] = productInfo;
            }

            return View();
        }

        private async Task<Dictionary<string, object>> GetProductFromDatabase(string productName)
        {
            var productInfo = new Dictionary<string, object>();

            try
            {
                // 使用 EF Core LINQ 查詢產品詳細資訊
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Prices)
                    .Where(p => p.Name == productName && p.IsActive)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    var mediumPrice = product.Prices.FirstOrDefault(pr => pr.SizeName == "中杯" && pr.IsActive);
                    var largePrice = product.Prices.FirstOrDefault(pr => pr.SizeName == "大杯" && pr.IsActive);

                    productInfo["Id"] = product.Id;
                    productInfo["Name"] = product.Name;
                    productInfo["Description"] = product.Description ?? "";
                    productInfo["ImageUrl"] = product.ImageUrl ?? "";
                    productInfo["IsSpecial"] = product.IsSpecial;
                    productInfo["CategoryName"] = product.Category.Name;
                    productInfo["MediumPrice"] = mediumPrice?.PriceAmount ?? 0;
                    productInfo["LargePrice"] = largePrice?.PriceAmount ?? 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢產品時發生錯誤");
            }

            return productInfo;
        }

        // 建立訂單（POST方法，接收表單資料）
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            string ProductName,
            string Size,
            int Quantity,
            decimal UnitPrice,
            decimal TotalAmount,
            string ImageName,
            string CustomerName,
            string CustomerPhone,
            string CustomerEmail,
            string DeliveryType,
            string CustomerAddress,
            string PaymentMethod,
            string Notes)
        {
            try
            {
                _logger.LogInformation("開始建立訂單 - 客戶: {CustomerName}, Email: {CustomerEmail}", CustomerName, CustomerEmail);

                // 解析商品資料（可能是單個商品或購物車商品列表）
                List<CartItem> cartItems = new List<CartItem>();

                try
                {
                    // 嘗試解析為 JSON（購物車多個商品）
                    cartItems = JsonSerializer.Deserialize<List<CartItem>>(ProductName) ?? new List<CartItem>();
                    _logger.LogInformation("成功解析購物車商品 JSON，共 {Count} 個項目", cartItems.Count);
                }
                catch
                {
                    // 如果解析失敗，表示是單個商品
                    _logger.LogInformation("解析為單一商品: {ProductName}", ProductName);
                    cartItems.Add(new CartItem
                    {
                        name = ProductName ?? "",
                        size = Size ?? "",
                        quantity = Quantity,
                        price = UnitPrice,
                        total = TotalAmount,
                        image = ImageName ?? ""
                    });
                }

                // 生成訂單編號（格式：ORD + 日期 + 流水號）
                string orderNumber = await GenerateOrderNumber();
                _logger.LogInformation("生成訂單編號: {OrderNumber}", orderNumber);

                // 使用 EF Core Transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 1. 建立訂單主表
                        var order = new Order
                        {
                            OrderNumber = orderNumber,
                            MemberId = null,
                            CustomerName = CustomerName ?? "",
                            CustomerPhone = CustomerPhone ?? "",
                            CustomerEmail = CustomerEmail,
                            CustomerAddress = CustomerAddress,
                            DeliveryType = DeliveryType,
                            Notes = Notes,
                            TotalAmount = TotalAmount,
                            OrderStatus = "待處理",
                            PaymentStatus = "未付款",
                            PaymentMethod = PaymentMethod,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("訂單主表已儲存，Order ID: {OrderId}", order.Id);

                        // 2. 為每個購物車商品插入訂單明細
                        foreach (var item in cartItems)
                        {
                            // 獲取產品ID（從產品名稱查詢）
                            var product = await _context.Products
                                .Where(p => p.Name == item.name)
                                .FirstOrDefaultAsync();

                            var orderDetail = new OrderDetail
                            {
                                OrderId = order.Id,
                                ProductId = product?.Id,
                                ProductName = item.name,
                                SizeName = item.size,
                                Quantity = item.quantity,
                                UnitPrice = item.price,
                                Subtotal = item.total,
                                CreatedAt = DateTime.UtcNow
                            };

                            _context.OrderDetails.Add(orderDetail);
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("訂單明細已儲存，共 {Count} 筆", cartItems.Count);

                        // 提交交易
                        await transaction.CommitAsync();
                        _logger.LogInformation("✅ 訂單交易已提交成功！訂單編號：{OrderNumber}", orderNumber);

                        // 發送訂單確認Email
                        if (!string.IsNullOrEmpty(CustomerEmail))
                        {
                            _logger.LogInformation("📧 準備發送Email到：{CustomerEmail}", CustomerEmail);
                            try
                            {
                                SendOrderConfirmationEmail(CustomerEmail, orderNumber, CustomerName ?? "",
                                    cartItems, TotalAmount, DeliveryType ?? "", CustomerAddress ?? "");
                                _logger.LogInformation("✅ Email發送完成");
                            }
                            catch (Exception emailEx)
                            {
                                _logger.LogError(emailEx, "❌ 發送Email時發生錯誤");
                                // 即使Email發送失敗，訂單已建立，仍然繼續
                            }
                        }

                        // 跳轉到訂單確認頁面
                        return RedirectToAction("OrderConfirmation", new { orderNumber = orderNumber });
                    }
                    catch (Exception ex)
                    {
                        // 發生錯誤，回滾交易
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "訂單建立失敗，交易已回滾");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立訂單時發生錯誤");
                TempData["ErrorMessage"] = $"訂單建立失敗：{ex.Message}";
                return RedirectToAction("Cart");
            }
        }

        // 生成訂單編號（格式：ORD + YYYYMMDD + 3位數流水號）
        private async Task<string> GenerateOrderNumber()
        {
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            string pattern = $"ORD{dateStr}";

            // 查詢今日最後一筆訂單編號
            var lastOrder = await _context.Orders
                .Where(o => o.OrderNumber.StartsWith(pattern))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrder != null)
            {
                // 取得最後的流水號並加1
                string lastOrderNumber = lastOrder.OrderNumber;
                int lastSeq = int.Parse(lastOrderNumber.Substring(11)); // ORD20250127001 取最後3位
                int newSeq = lastSeq + 1;
                return $"ORD{dateStr}{newSeq:D3}";
            }
            else
            {
                // 今日第一筆訂單
                return $"ORD{dateStr}001";
            }
        }

        // 發送訂單確認Email
        private void SendOrderConfirmationEmail(string toEmail, string orderNumber, string customerName,
            List<CartItem> cartItems, decimal totalAmount, string deliveryType, string address)
        {
            _logger.LogInformation("========== 開始Email發送流程 ==========");
            _logger.LogInformation("收件者：{ToEmail}", toEmail);
            _logger.LogInformation("訂單編號：{OrderNumber}", orderNumber);

            try
            {
                string subject = $"【品渴茶鋪】訂單確認通知 - {orderNumber}";
                _logger.LogInformation("Email主旨：{Subject}", subject);

                // 建立商品列表HTML
                string productsHtml = "";
                foreach (var item in cartItems)
                {
                    productsHtml += $@"
                <div class='info-row'>
                    <span class='label'>{item.name} ({item.size})</span>
                    <span class='value'>{item.quantity} 杯 × NT$ {item.price} = NT$ {item.total}</span>
                </div>";
                }

                string body = $@"
<html>
<head>
    <style>
        body {{ font-family: 'Microsoft JhengHei', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f9f9f9; }}
        .header {{ background: linear-gradient(135deg, #FFB6C1, #FF69B4); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: white; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-info {{ background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; }}
        .info-row {{ display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #e0e0e0; }}
        .label {{ font-weight: bold; color: #666; }}
        .value {{ color: #333; }}
        .total {{ font-size: 1.3em; font-weight: bold; color: #FF69B4; margin-top: 15px; text-align: right; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 0.9em; }}
        .icon {{ color: #FF69B4; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>✅ 訂單確認通知</h1>
            <p>感謝您的訂購！</p>
        </div>
        <div class='content'>
            <p>親愛的 <strong>{customerName}</strong> 您好，</p>
            <p>您的訂單已成功送出，我們將盡快為您製作美味的飲品！</p>

            <div class='order-info'>
                <h3 style='color: #FF69B4; margin-top: 0;'>📋 訂單資訊</h3>
                <div class='info-row'>
                    <span class='label'>訂單編號：</span>
                    <span class='value'>{orderNumber}</span>
                </div>
                <h4 style='margin-top: 20px; margin-bottom: 10px;'>訂購商品：</h4>
                {productsHtml}
                <div class='info-row'>
                    <span class='label'>取貨方式：</span>
                    <span class='value'>{deliveryType}</span>
                </div>
                {(deliveryType == "外送" && !string.IsNullOrEmpty(address) ? $@"
                <div class='info-row'>
                    <span class='label'>外送地址：</span>
                    <span class='value'>{address}</span>
                </div>" : "")}
                <div class='total'>
                    訂單總金額：NT$ {totalAmount}
                </div>
            </div>

            <div style='background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin-top: 20px; border-radius: 5px;'>
                <h4 style='margin-top: 0; color: #856404;'>📌 溫馨提醒</h4>
                <ul style='margin: 0; padding-left: 20px; color: #856404;'>
                    <li>請保留此訂單編號以便查詢訂單狀態</li>
                    <li>我們將盡快為您製作飲品</li>
                    <li>{(deliveryType == "自取" ? "請在訂單完成後到店自取" : "我們將盡快為您安排外送")}</li>
                    <li>如有任何問題，請隨時與我們聯繫</li>
                </ul>
            </div>

            <div class='footer'>
                <p><strong>品渴茶鋪</strong></p>
                <p>感謝您的光臨，期待再次為您服務！</p>
                <p style='font-size: 0.8em; color: #999;'>此為系統自動發送的郵件，請勿直接回覆</p>
            </div>
        </div>
    </div>
</body>
</html>";

                // 從配置文件讀取Email設定
                _logger.LogInformation("📖 讀取Email設定...");
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                _logger.LogInformation("SMTP伺服器：{SmtpHost}:{SmtpPort}", smtpHost, smtpPort);
                _logger.LogInformation("寄件者：{SenderEmail}", senderEmail);
                _logger.LogInformation("登入帳號：{Username}", username);
                _logger.LogInformation("密碼長度：{PasswordLength} 字元", password?.Length ?? 0);

                // 檢查是否已配置Email設定
                if (string.IsNullOrEmpty(username) || username.Contains("請改成") ||
                    string.IsNullOrEmpty(password) || password.Contains("your-"))
                {
                    _logger.LogWarning("⚠️ Email設定尚未配置，請在appsettings.json中設定您的Gmail帳號和應用程式密碼");
                    throw new Exception("Email設定尚未配置");
                }

                _logger.LogInformation("✅ Email設定檢查通過");

                _logger.LogInformation("📝 建立郵件訊息...");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail ?? "", senderName);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;
                _logger.LogInformation("✅ 郵件訊息建立完成");

                // SMTP設定（從appsettings.json讀取）
                _logger.LogInformation("🔧 設定SMTP客戶端...");
                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpHost ?? "smtp.gmail.com";
                smtp.Port = smtpPort;
                smtp.EnableSsl = enableSsl;
                smtp.Credentials = new NetworkCredential(username, password);
                _logger.LogInformation("✅ SMTP客戶端設定完成");

                _logger.LogInformation("📤 開始發送Email...");
                smtp.Send(mail);
                _logger.LogInformation("✅✅✅ 訂單確認Email已成功發送至：{ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email發送失敗");
                throw;
            }
        }

        // 訂單確認頁面
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return RedirectToAction("Index");
            }

            // 從資料庫查詢訂單資訊
            var orderInfo = await GetOrderByNumber(orderNumber);

            if (orderInfo == null)
            {
                TempData["ErrorMessage"] = "找不到訂單資訊";
                return RedirectToAction("Index");
            }

            ViewData["OrderInfo"] = orderInfo;
            return View();
        }

        // 根據訂單編號查詢訂單詳細資訊
        private async Task<Dictionary<string, object>?> GetOrderByNumber(string orderNumber)
        {
            var orderInfo = new Dictionary<string, object>();

            try
            {
                // 使用 EF Core 查詢訂單主資訊及明細
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Where(o => o.OrderNumber == orderNumber)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return null;
                }

                orderInfo["OrderNumber"] = order.OrderNumber;
                orderInfo["CustomerName"] = order.CustomerName;
                orderInfo["CustomerPhone"] = order.CustomerPhone;
                orderInfo["CustomerEmail"] = order.CustomerEmail ?? "";
                orderInfo["CustomerAddress"] = order.CustomerAddress ?? "";
                orderInfo["DeliveryType"] = order.DeliveryType ?? "";
                orderInfo["Notes"] = order.Notes ?? "";
                orderInfo["TotalAmount"] = order.TotalAmount;
                orderInfo["OrderStatus"] = order.OrderStatus;
                orderInfo["PaymentStatus"] = order.PaymentStatus;
                orderInfo["PaymentMethod"] = order.PaymentMethod ?? "";
                orderInfo["CreatedAt"] = order.CreatedAt;
                orderInfo["OrderId"] = order.Id;

                // 訂單明細
                var orderDetails = order.OrderDetails.Select(od => new Dictionary<string, object>
                {
                    ["ProductName"] = od.ProductName,
                    ["SizeName"] = od.SizeName,
                    ["Quantity"] = od.Quantity,
                    ["UnitPrice"] = od.UnitPrice,
                    ["Subtotal"] = od.Subtotal,
                    ["SweetnessLevel"] = od.SweetnessLevel ?? "",
                    ["IceLevel"] = od.IceLevel ?? ""
                }).ToList();

                orderInfo["OrderDetails"] = orderDetails;

                return orderInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢訂單時發生錯誤");
                return null;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
