using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PinkshopDbContext _context;

        public OrdersController(
            ILogger<OrdersController> logger,
            IConfiguration configuration,
            PinkshopDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        // API 模型定義
        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string Size { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public string? Image { get; set; }
        }

        public class CreateOrderDto
        {
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public string? DeliveryAddress { get; set; }
            public string DeliveryType { get; set; } = "自取";
            public string PaymentMethod { get; set; } = "貨到付款";
            public string? Notes { get; set; }
            public List<OrderItemDto> Items { get; set; } = new();
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto request)
        {
            try
            {
                _logger.LogInformation("=== 開始建立訂單 ===");
                _logger.LogInformation("客戶姓名: {Name}", request.CustomerName);
                _logger.LogInformation("客戶Email: {Email}", request.CustomerEmail);
                _logger.LogInformation("商品數量: {Count}", request.Items?.Count ?? 0);

                // 驗證請求
                if (string.IsNullOrEmpty(request.CustomerName) ||
                    string.IsNullOrEmpty(request.CustomerEmail) ||
                    string.IsNullOrEmpty(request.CustomerPhone))
                {
                    return BadRequest(new { success = false, message = "請填寫完整的客戶資訊" });
                }

                if (request.Items == null || !request.Items.Any())
                {
                    return BadRequest(new { success = false, message = "購物車是空的" });
                }

                // 生成訂單編號
                string orderNumber = await GenerateOrderNumber();
                _logger.LogInformation("生成訂單編號: {OrderNumber}", orderNumber);

                // 計算總金額
                decimal totalAmount = request.Items.Sum(item => item.Price * item.Quantity);

                // 使用交易
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 創建訂單主記錄
                        var order = new Order
                        {
                            OrderNumber = orderNumber,
                            CustomerName = request.CustomerName,
                            CustomerPhone = request.CustomerPhone,
                            CustomerEmail = request.CustomerEmail,
                            CustomerAddress = request.DeliveryAddress,
                            TotalAmount = totalAmount,
                            OrderStatus = "待處理",
                            PaymentStatus = "未付款",
                            PaymentMethod = request.PaymentMethod,
                            DeliveryType = request.DeliveryType,
                            Notes = request.Notes,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Orders.Add(order);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation("✅ 訂單主記錄已建立，訂單ID: {OrderId}", order.Id);

                        // 創建訂單明細
                        foreach (var item in request.Items)
                        {
                            _logger.LogInformation("處理訂單明細 - 產品: {ProductName}, 尺寸: {Size}, 數量: {Quantity}, 價格: {Price}",
                                item.ProductName, item.Size, item.Quantity, item.Price);

                            var orderDetail = new OrderDetail
                            {
                                OrderId = order.Id,
                                ProductName = item.ProductName ?? "未知商品",
                                SizeName = item.Size ?? "中杯",
                                Quantity = item.Quantity,
                                UnitPrice = item.Price,
                                Subtotal = item.Price * item.Quantity,
                                CreatedAt = DateTime.UtcNow
                            };

                            _context.OrderDetails.Add(orderDetail);
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("✅ 訂單明細已建立");

                        // 提交交易
                        await transaction.CommitAsync();
                        _logger.LogInformation("✅ 交易已提交");

                        // 在背景發送確認郵件（不阻塞響應）
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await SendOrderConfirmationEmail(
                                    request.CustomerEmail,
                                    request.CustomerName,
                                    orderNumber,
                                    request.Items,
                                    totalAmount,
                                    request.DeliveryType,
                                    request.DeliveryAddress ?? ""
                                );
                                _logger.LogInformation("✅ 確認郵件已發送");
                            }
                            catch (Exception emailEx)
                            {
                                _logger.LogError(emailEx, "❌ 發送郵件失敗（但訂單已建立）");
                                // 郵件失敗不影響訂單建立
                            }
                        });

                        _logger.LogInformation("✅ 訂單建立成功，Email將在背景發送");

                        return Ok(new
                        {
                            success = true,
                            message = "訂單建立成功",
                            orderNumber = orderNumber,
                            orderId = order.Id
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "❌ 建立訂單失敗，交易已回滾");
                        _logger.LogError("錯誤詳情: {Message}", ex.Message);
                        if (ex.InnerException != null)
                        {
                            _logger.LogError("內部錯誤: {InnerMessage}", ex.InnerException.Message);
                        }
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateOrder 發生錯誤");
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | 內部錯誤: " + ex.InnerException.Message;
                }
                return StatusCode(500, new
                {
                    success = false,
                    message = "訂單建立失敗：" + errorMessage
                });
            }
        }

        // 生成訂單編號
        private async Task<string> GenerateOrderNumber()
        {
            string datePrefix = DateTime.Now.ToString("yyyyMMdd");
            string pattern = $"ORD{datePrefix}%";

            var todayOrderCount = await _context.Orders
                .Where(o => EF.Functions.Like(o.OrderNumber, pattern))
                .CountAsync();

            int nextNumber = todayOrderCount + 1;
            return $"ORD{datePrefix}{nextNumber:D4}";
        }

        // 發送訂單確認郵件
        private async Task SendOrderConfirmationEmail(
            string toEmail,
            string customerName,
            string orderNumber,
            List<OrderItemDto> items,
            decimal totalAmount,
            string deliveryType,
            string address)
        {
            try
            {
                _logger.LogInformation("📧 開始發送訂單確認郵件...");

                // 建立商品列表 HTML
                var productsHtml = string.Join("", items.Select(item => $@"
                    <div class='info-row'>
                        <span class='label'>{item.ProductName} ({item.Size})</span>
                        <span class='value'>{item.Quantity} x NT$ {item.Price} = NT$ {item.Price * item.Quantity}</span>
                    </div>"));

                string subject = $"【品渴茶鋪】訂單確認通知 - {orderNumber}";
                string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
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

                // 讀取 Email 設定
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];

                _logger.LogInformation("SMTP: {Host}:{Port}, SSL: {Ssl}", smtpHost, smtpPort, enableSsl);

                // 建立郵件
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail ?? "", senderName);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    // SMTP 設定
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = smtpHost ?? "smtp.gmail.com";
                        smtp.Port = smtpPort;
                        smtp.EnableSsl = enableSsl;
                        smtp.Credentials = new NetworkCredential(username, password);
                        smtp.Timeout = 10000; // 10秒超時

                        await Task.Run(() => smtp.Send(mail));
                    }
                }

                _logger.LogInformation("✅ 郵件發送成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 發送郵件失敗");
                throw;
            }
        }

        // GET api/orders/{orderNumber}
        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetOrder(string orderNumber)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

                if (order == null)
                {
                    return NotFound(new { success = false, message = "找不到訂單" });
                }

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        orderNumber = order.OrderNumber,
                        customerName = order.CustomerName,
                        customerEmail = order.CustomerEmail,
                        customerPhone = order.CustomerPhone,
                        totalAmount = order.TotalAmount,
                        orderStatus = order.OrderStatus,
                        paymentStatus = order.PaymentStatus,
                        deliveryType = order.DeliveryType,
                        createdAt = order.CreatedAt,
                        items = order.OrderDetails.Select(od => new
                        {
                            productName = od.ProductName,
                            size = od.SizeName,
                            quantity = od.Quantity,
                            unitPrice = od.UnitPrice,
                            subtotal = od.Subtotal
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢訂單失敗");
                return StatusCode(500, new { success = false, message = "查詢訂單失敗" });
            }
        }
    }
}
