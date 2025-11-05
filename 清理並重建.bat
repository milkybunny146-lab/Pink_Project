@echo off
echo ========================================
echo 清理專案並強制重新編譯
echo ========================================
echo.

cd /d "%~dp0"

echo [1/4] 停止所有運行中的程序...
timeout /t 2 /nobreak >nul

echo [2/4] 刪除 bin 資料夾...
if exist "WebApplication1\bin" (
    rmdir /s /q "WebApplication1\bin"
    echo     ✓ bin 資料夾已刪除
) else (
    echo     ℹ bin 資料夾不存在
)

echo [3/4] 刪除 obj 資料夾...
if exist "WebApplication1\obj" (
    rmdir /s /q "WebApplication1\obj"
    echo     ✓ obj 資料夾已刪除
) else (
    echo     ℹ obj 資料夾不存在
)

echo [4/4] 清理完成！
echo.
echo ========================================
echo ✅ 清理成功！
echo ========================================
echo.
echo 接下來請執行以下步驟：
echo 1. 在 Visual Studio 按 F5 重新啟動應用程式
echo 2. 在瀏覽器按 Ctrl+Shift+N 開啟無痕視窗
echo 3. 訪問 localhost:7184/Home/Cart
echo.
pause

