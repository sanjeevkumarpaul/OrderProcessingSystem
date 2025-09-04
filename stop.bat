@echo off
REM Launcher batch file for Windows
REM File: stop.bat

echo 🖥️ OrderProcessingSystem - Windows Stop Launcher
echo Redirecting to WINOS/stop.bat...
echo.

REM Check if WINOS folder and stop.bat exist
if not exist "WINOS\stop.bat" (
    echo ❌ Error: WINOS\stop.bat not found!
    echo Make sure you're running this from the OrderProcessingSystem root directory.
    pause
    exit /b 1
)

REM Run the Windows batch script
call "WINOS\stop.bat"
