using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

// 添加服務到容器
builder.Services.AddControllersWithViews();

// ⭐ 關鍵：添加 API Controllers 支援
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 設定 JSON 序列化選項（可選）
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// ⭐ 添加 CORS 支援（允許前端呼叫 API）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 添加資料庫上下文
builder.Services.AddDbContext<PinkshopDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// 添加 Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// 配置 HTTP 請求管道
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ⭐ 使用 CORS（必須在 UseRouting 之前）
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// ⭐ 映射 MVC Controllers（用於網頁）
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ⭐ 映射 API Controllers（用於 API 端點）
app.MapControllers();

// 輸出啟動資訊
app.Logger.LogInformation("=== 品渴茶鋪 Web API 已啟動 ===");
app.Logger.LogInformation("環境: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("API 端點已啟用：");
app.Logger.LogInformation("  - GET  /api/products");
app.Logger.LogInformation("  - GET  /api/products/{id}");
app.Logger.LogInformation("  - POST /api/orders");
app.Logger.LogInformation("  - GET  /api/orders/{orderNumber}");

app.Run();