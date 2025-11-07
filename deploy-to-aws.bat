@echo off
echo ========================================
echo   品渴茶鋪 - AWS Elastic Beanstalk 部署
echo ========================================
echo.

REM 檢查是否已安裝 EB CLI
eb --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [錯誤] 未安裝 EB CLI
    echo 請先執行: pip install awsebcli --upgrade
    pause
    exit /b 1
)

echo [1/4] 檢查 Git 狀態...
git status

echo.
echo [2/4] 提交變更到 Git...
set /p commit_message="請輸入 commit 訊息 (或按 Enter 使用預設): "
if "%commit_message%"=="" set commit_message=更新應用程式

git add .
git commit -m "%commit_message%"

echo.
echo [3/4] 部署到 AWS Elastic Beanstalk...
eb deploy

echo.
echo [4/4] 檢查部署狀態...
eb status

echo.
echo ========================================
echo   部署完成！
echo ========================================
echo.
echo 執行以下指令來查看應用程式：
echo   eb open
echo.
echo 查看日誌：
echo   eb logs
echo.
pause
