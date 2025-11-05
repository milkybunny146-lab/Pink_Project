using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class DbSeeder
    {
        private readonly PinkshopDbContext _context;

        public DbSeeder(PinkshopDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // 檢查資料庫是否已有資料
            if (_context.Categories.Any())
            {
                Console.WriteLine("資料庫已包含資料，跳過種子資料插入。");
                return;
            }

            Console.WriteLine("開始插入初始資料...");

            // 1. 插入分類
            var categories = new List<Category>
            {
                new Category { Name = "烤奶系列", DisplayOrder = 1, IsActive = true },
                new Category { Name = "養生燉品", DisplayOrder = 2, IsActive = true }
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            Console.WriteLine("✓ 分類資料已插入");

            // 2. 插入產品
            var products = new List<Product>
            {
                new Product
                {
                    CategoryId = categories[0].Id,
                    Name = "圍爐奶茶",
                    Description = "經典奶茶香氣濃郁",
                    IsSpecial = false,
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Product
                {
                    CategoryId = categories[0].Id,
                    Name = "重瓣玫瑰烤奶",
                    Description = "芬芳玫瑰與奶香完美結合",
                    IsSpecial = false,
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Product
                {
                    CategoryId = categories[0].Id,
                    Name = "經典桂花烤奶",
                    Description = "桂花香甜細緻",
                    IsSpecial = false,
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Product
                {
                    CategoryId = categories[0].Id,
                    Name = "頂級茉香烤奶",
                    Description = "茉莉花香清新優雅",
                    IsSpecial = false,
                    DisplayOrder = 4,
                    IsActive = true
                },
                new Product
                {
                    CategoryId = categories[0].Id,
                    Name = "馥郁薰衣草烤奶",
                    Description = "薰衣草香氣舒緩放鬆",
                    IsSpecial = false,
                    DisplayOrder = 5,
                    IsActive = true
                },
                new Product
                {
                    CategoryId = categories[1].Id,
                    Name = "桃膠雪燕燉奶",
                    Description = "養顏美容聖品",
                    IsSpecial = true,
                    DisplayOrder = 1,
                    IsActive = true
                }
            };
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            Console.WriteLine("✓ 產品資料已插入");

            // 3. 插入價格
            var prices = new List<Price>
            {
                // 圍爐奶茶
                new Price { ProductId = products[0].Id, SizeName = "中杯", PriceAmount = 70.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[0].Id, SizeName = "大杯", PriceAmount = 90.00M, DisplayOrder = 2, IsActive = true },

                // 重瓣玫瑰烤奶
                new Price { ProductId = products[1].Id, SizeName = "中杯", PriceAmount = 90.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[1].Id, SizeName = "大杯", PriceAmount = 120.00M, DisplayOrder = 2, IsActive = true },

                // 經典桂花烤奶
                new Price { ProductId = products[2].Id, SizeName = "中杯", PriceAmount = 90.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[2].Id, SizeName = "大杯", PriceAmount = 120.00M, DisplayOrder = 2, IsActive = true },

                // 頂級茉香烤奶
                new Price { ProductId = products[3].Id, SizeName = "中杯", PriceAmount = 90.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[3].Id, SizeName = "大杯", PriceAmount = 120.00M, DisplayOrder = 2, IsActive = true },

                // 馥郁薰衣草烤奶
                new Price { ProductId = products[4].Id, SizeName = "中杯", PriceAmount = 90.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[4].Id, SizeName = "大杯", PriceAmount = 120.00M, DisplayOrder = 2, IsActive = true },

                // 桃膠雪燕燉奶
                new Price { ProductId = products[5].Id, SizeName = "中杯", PriceAmount = 120.00M, DisplayOrder = 1, IsActive = true },
                new Price { ProductId = products[5].Id, SizeName = "大杯", PriceAmount = 150.00M, DisplayOrder = 2, IsActive = true }
            };
            await _context.Prices.AddRangeAsync(prices);
            await _context.SaveChangesAsync();
            Console.WriteLine("✓ 價格資料已插入");

            // 4. 插入甜度選項
            var sweetnessLevels = new List<SweetnessLevel>
            {
                new SweetnessLevel { Name = "正常", DisplayOrder = 1, IsActive = true },
                new SweetnessLevel { Name = "少糖", DisplayOrder = 2, IsActive = true },
                new SweetnessLevel { Name = "微糖", DisplayOrder = 3, IsActive = true },
                new SweetnessLevel { Name = "無糖", DisplayOrder = 4, IsActive = true }
            };
            await _context.SweetnessLevels.AddRangeAsync(sweetnessLevels);
            await _context.SaveChangesAsync();
            Console.WriteLine("✓ 甜度選項已插入");

            // 5. 插入冰度選項
            var iceLevels = new List<IceLevel>
            {
                new IceLevel { Name = "正常冰", DisplayOrder = 1, IsActive = true },
                new IceLevel { Name = "少冰", DisplayOrder = 2, IsActive = true },
                new IceLevel { Name = "微冰", DisplayOrder = 3, IsActive = true },
                new IceLevel { Name = "去冰", DisplayOrder = 4, IsActive = true },
                new IceLevel { Name = "溫", DisplayOrder = 5, IsActive = true },
                new IceLevel { Name = "熱", DisplayOrder = 6, IsActive = true }
            };
            await _context.IceLevels.AddRangeAsync(iceLevels);
            await _context.SaveChangesAsync();
            Console.WriteLine("✓ 冰度選項已插入");

            Console.WriteLine("========================================");
            Console.WriteLine("初始資料插入完成！");
            Console.WriteLine($"- 分類: {categories.Count} 筆");
            Console.WriteLine($"- 產品: {products.Count} 筆");
            Console.WriteLine($"- 價格: {prices.Count} 筆");
            Console.WriteLine($"- 甜度: {sweetnessLevels.Count} 筆");
            Console.WriteLine($"- 冰度: {iceLevels.Count} 筆");
            Console.WriteLine("========================================");
        }
    }
}
