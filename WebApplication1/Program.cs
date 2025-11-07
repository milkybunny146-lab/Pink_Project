using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// ⭐ 新增：設定監聽所有網絡介面的 port 80
builder.WebHost.UseUrls("http://0.0.0.0:80");

// 註冊 DbContext
builder.Services.AddDbContext<PinkshopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 MVC 服務（用於網頁）
builder.Services.AddControllersWithViews();

// 添加 API Controllers 支援
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 添加 CORS 支援
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 添加 Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ⭐ 修改：使用環境變數控制 Seed（而非只在 Development 環境）
var shouldSeed = builder.Configuration.GetValue<bool>("EnableDbSeed", false);
if (shouldSeed)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<PinkshopDbContext>();
            var seeder = new DbSeeder(context);
            await seeder.SeedAsync();
            app.Logger.LogInformation("資料庫種子資料已成功插入");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "種子資料插入時發生錯誤");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// 輸出啟動資訊
app.Logger.LogInformation("=== 品渴茶鋪 Web API 已啟動 ===");
app.Logger.LogInformation("環境: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("資料庫連接字串: {ConnectionString}", 
    builder.Configuration.GetConnectionString("DefaultConnection")?.Substring(0, 30) + "...");
app.Logger.LogInformation("API 端點已啟用：");
app.Logger.LogInformation("  - GET  /api/products");
app.Logger.LogInformation("  - POST /api/orders");

app.Run();