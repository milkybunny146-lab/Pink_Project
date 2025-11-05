using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly PinkshopDbContext _context;

        public ProductsController(ILogger<ProductsController> logger, PinkshopDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                _logger.LogInformation("查詢所有產品...");

                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Prices)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Category.DisplayOrder)
                    .ThenBy(p => p.Id)
                    .Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        description = p.Description,
                        imageUrl = p.ImageUrl,
                        isSpecial = p.IsSpecial,
                        category = new
                        {
                            id = p.Category.Id,
                            name = p.Category.Name
                        },
                        prices = p.Prices
                            .Where(pr => pr.IsActive)
                            .OrderBy(pr => pr.SizeName)
                            .Select(pr => new
                            {
                                id = pr.Id,
                                sizeName = pr.SizeName,
                                price = pr.PriceAmount
                            })
                            .ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation("找到 {Count} 個產品", products.Count);

                return Ok(new
                {
                    success = true,
                    data = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢產品失敗");
                return StatusCode(500, new
                {
                    success = false,
                    message = "查詢產品失敗：" + ex.Message
                });
            }
        }

        // GET api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Prices)
                    .Where(p => p.Id == id && p.IsActive)
                    .Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        description = p.Description,
                        imageUrl = p.ImageUrl,
                        isSpecial = p.IsSpecial,
                        category = new
                        {
                            id = p.Category.Id,
                            name = p.Category.Name
                        },
                        prices = p.Prices
                            .Where(pr => pr.IsActive)
                            .OrderBy(pr => pr.SizeName)
                            .Select(pr => new
                            {
                                id = pr.Id,
                                sizeName = pr.SizeName,
                                price = pr.PriceAmount
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "找不到產品"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = product
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢產品失敗");
                return StatusCode(500, new
                {
                    success = false,
                    message = "查詢產品失敗：" + ex.Message
                });
            }
        }

        // GET api/products/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Prices)
                    .Where(p => p.CategoryId == categoryId && p.IsActive)
                    .OrderBy(p => p.Id)
                    .Select(p => new
                    {
                        id = p.Id,
                        name = p.Name,
                        description = p.Description,
                        imageUrl = p.ImageUrl,
                        isSpecial = p.IsSpecial,
                        category = new
                        {
                            id = p.Category.Id,
                            name = p.Category.Name
                        },
                        prices = p.Prices
                            .Where(pr => pr.IsActive)
                            .OrderBy(pr => pr.SizeName)
                            .Select(pr => new
                            {
                                id = pr.Id,
                                sizeName = pr.SizeName,
                                price = pr.PriceAmount
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "查詢產品失敗");
                return StatusCode(500, new
                {
                    success = false,
                    message = "查詢產品失敗：" + ex.Message
                });
            }
        }
    }
}
