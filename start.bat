@echo off
REM Launcher batch file for Windows
REM File: start.bat

echo 🖥️ OrderProcessingSystem - Windows Launcher
echo Redirecting to WINOS/start.bat...
echo.

REM Check if WINOS folder and start.bat exist
if not exist "WINOS\start.bat" (
    echo ❌ Error: WINOS\start.bat not found!
    echo Make sure you're running this from the OrderProcessingSystem root directory.
    pause
    exit /b 1
)

REM Run the Windows batch script
call "WINOS\start.bat"
