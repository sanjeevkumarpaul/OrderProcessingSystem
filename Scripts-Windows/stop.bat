@echo off
REM Batch file to run the PowerShell stop script
REM File: stop.bat

echo Stopping OrderProcessingSystem services...
echo.

REM Check if PowerShell is available
where powershell >nul 2>&1
if errorlevel 1 (
    echo ERROR: PowerShell is not found in PATH
    echo Please ensure PowerShell is installed and accessible
    pause
    exit /b 1
)

REM Run the PowerShell script from parent directory
cd /d "%~dp0.."
powershell -ExecutionPolicy Bypass -File "WINOS/stop.ps1"

REM Keep window open if there was an error
if errorlevel 1 (
    echo.
    echo Script execution failed. Check the error messages above.
    pause
)
