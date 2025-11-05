using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// 註冊 DbContext
builder.Services.AddDbContext<PinkshopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 MVC 服務（用於網頁）
builder.Services.AddControllersWithViews();

// ⭐ 新增：添加 API Controllers 支援
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 設定 JSON 序列化選項
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ⭐ 新增：添加 CORS 支援（允許前端呼叫 API）
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

// 執行資料庫種子資料插入（僅在開發環境）
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<PinkshopDbContext>();
            var seeder = new DbSeeder(context);
            await seeder.SeedAsync();
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

// ⭐ 新增：使用 CORS（必須在 UseRouting 之前）
app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 映射 MVC 路由（用於網頁）
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ⭐ 新增：映射 API Controllers 路由
app.MapControllers();

// 輸出啟動資訊
app.Logger.LogInformation("=== 品渴茶鋪 Web API 已啟動 ===");
app.Logger.LogInformation("環境: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("API 端點已啟用：");
app.Logger.LogInformation("  - GET  /api/products");
app.Logger.LogInformation("  - POST /api/orders");

app.Run();