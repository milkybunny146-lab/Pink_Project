using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly PinkshopDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(PinkshopDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/Menu
        public IActionResult Menu()
        {
            return View();
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // GET: Home/News
        public IActionResult News()
        {
            return View();
        }

        // GET: Home/Cart
        public IActionResult Cart(string item, int? mediumPrice, int? largePrice, string? image)
        {
            ViewData["ItemName"] = item ?? "圍爐奶茶";
            ViewData["MediumPrice"] = mediumPrice?.ToString() ?? "70";
            ViewData["LargePrice"] = largePrice?.ToString() ?? "90";
            ViewData["ImageName"] = image ?? "orginal.jpg";

            return View();
        }

        // GET: Home/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("訂單編號不能為空");
            }

            try
            {
                // 從資料庫查詢訂單
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

                if (order == null)
                {
                    return NotFound("找不到訂單");
                }

                // 準備訂單資訊傳給View
                var orderInfo = new Dictionary<string, object>
                {
                    ["OrderNumber"] = order.OrderNumber,
                    ["CustomerName"] = order.CustomerName,
                    ["CustomerPhone"] = order.CustomerPhone,
                    ["CustomerEmail"] = order.CustomerEmail ?? "",
                    ["CustomerAddress"] = order.CustomerAddress ?? "",
                    ["DeliveryType"] = order.DeliveryType ?? "自取",
                    ["Notes"] = order.Notes ?? "",
                    ["TotalAmount"] = order.TotalAmount,
                    ["OrderStatus"] = order.OrderStatus,
                    ["PaymentStatus"] = order.PaymentStatus,
                    ["PaymentMethod"] = order.PaymentMethod ?? "貨到付款",
                    ["CreatedAt"] = order.CreatedAt,
                    ["OrderDetails"] = order.OrderDetails.Select(od => new Dictionary<string, object>
                    {
                        ["ProductName"] = od.ProductName,
                        ["SizeName"] = od.SizeName ?? "",
                        ["Quantity"] = od.Quantity,
                        ["UnitPrice"] = od.UnitPrice,
                        ["Subtotal"] = od.Subtotal
                    }).ToList()
                };

                ViewData["OrderInfo"] = orderInfo;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢訂單失敗: {OrderNumber}", orderNumber);
                return StatusCode(500, "查詢訂單時發生錯誤");
            }
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
