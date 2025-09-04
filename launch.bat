@echo off
REM Universal Cross-Platform Launcher for OrderProcessingSystem (Windows)
REM File: launch.bat

echo 🌍 OrderProcessingSystem - Universal Cross-Platform Launcher (Windows)
echo.

REM Get the action from parameter or default to start
set "action=%1"

REM Handle short flags and long parameters
if "%action%"=="--s" set "action=start"
if "%action%"=="--start" set "action=start"
if "%action%"=="--x" set "action=stop"
if "%action%"=="--stop" set "action=stop"
if "%action%"=="--q" set "action=start"
if "%action%"=="--qstart" set "action=start"
if "%action%"=="--quick" set "action=start"
if "%action%"=="qstart" set "action=start"

REM Default to start if no action specified
if "%action%"=="" set "action=start"

echo 🖥️  Detected: Windows
echo 🎯 Action: %action%
echo 📂 Using WINOS folder for Windows system...
echo.

REM Validate action
if "%action%"=="start" goto :execute
if "%action%"=="stop" goto :execute

echo ❌ Invalid action: %action%
echo Valid actions: start, stop (or use short flags --s, --x)
echo.
goto :help

:execute
REM Check if the required files exist
if not exist "WINOS\%action%.bat" (
    echo ❌ Error: WINOS\%action%.bat not found!
    echo Make sure you're running this from the OrderProcessingSystem root directory.
    pause
    exit /b 1
)

echo ▶️  Executing: WINOS\%action%.bat
echo.
call "WINOS\%action%.bat"
goto :end

:help
echo 🚀 OrderProcessingSystem Universal Launcher (Windows)
echo.
echo Usage:
echo   launch.bat [action]
echo.
echo Actions:
echo   start, --s, --start      - Start all services (default)
echo   stop, --x, --stop        - Stop all services
echo.
echo Short Flags:
echo   --s                      - Start services
echo   --x                      - Stop services
echo.
echo Examples:
echo   launch.bat               # Start services (default)
echo   launch.bat start         # Start services
echo   launch.bat --s           # Start services (short flag)
echo   launch.bat stop          # Stop services
echo   launch.bat --x           # Stop services (short flag)
echo.
echo Note: Quick start (--q) is not available on Windows
echo.
pause
exit /b 0

:end
