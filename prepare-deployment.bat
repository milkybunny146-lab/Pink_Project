@echo off
chcp 65001 >nul
echo ========================================
echo   品渴茶鋪 - AWS 部署檔案準備工具
echo ========================================
echo.

echo [1/3] 清理舊的發布檔案...
if exist "WebApplication1\publish" (
    rmdir /s /q "WebApplication1\publish"
)
if exist "pinkshop-deployment.zip" (
    del /f /q "pinkshop-deployment.zip"
)

echo [2/3] 建立發布版本...
cd WebApplication1
dotnet publish -c Release -o publish
if %errorlevel% neq 0 (
    echo.
    echo [錯誤] 發布失敗！
    pause
    exit /b 1
)

echo [3/3] 建立部署 ZIP 檔案...
cd publish
powershell -command "Compress-Archive -Path * -DestinationPath ..\..\pinkshop-deployment.zip -Force"
cd ..\..

echo.
echo ========================================
echo   ✅ 部署檔案準備完成！
echo ========================================
echo.
echo 檔案位置：
echo   %CD%\pinkshop-deployment.zip
echo.
echo 檔案大小：
for %%A in ("pinkshop-deployment.zip") do echo   %%~zA bytes
echo.
echo ========================================
echo   接下來的步驟：
echo ========================================
echo.
echo 1. 登入 AWS Console：
echo    https://console.aws.amazon.com/
echo.
echo 2. 前往 Elastic Beanstalk 服務
echo.
echo 3. 點擊「建立應用程式」
echo.
echo 4. 上傳 pinkshop-deployment.zip
echo.
echo 詳細步驟請參考：AWS_CONSOLE_DEPLOYMENT.md
echo.
pause
