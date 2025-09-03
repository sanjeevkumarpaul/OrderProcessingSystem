# PowerShell script to start OrderProcessingSystem projects in separate windows
# File: start-services.ps1

Write-Host "🚀 Starting OrderProcessingSystem Services..." -ForegroundColor Green
Write-Host "This script will launch 3 services in separate PowerShell windows:" -ForegroundColor Yellow
Write-Host "  1. Authentication Service (Port 5270)" -ForegroundColor Cyan
Write-Host "  2. API Service (Port 5000)" -ForegroundColor Cyan  
Write-Host "  3. UI Service (Port 5253)" -ForegroundColor Cyan
Write-Host ""

# Get the current directory (should be the root OrderProcessingSystem folder)
$rootPath = Get-Location
Write-Host "Root Path: $rootPath" -ForegroundColor Gray

# Define project paths
$authPath = Join-Path $rootPath "OrderProcessingSystem.Authentication"
$apiPath = Join-Path $rootPath "OrderProcessingSystem.API"
$uiPath = Join-Path $rootPath "OrderProcessingSystem.UI"

# Function to check if directory exists
function Test-ProjectPath {
    param($path, $name)
    if (Test-Path $path) {
        Write-Host "✅ Found: $name at $path" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ Missing: $name at $path" -ForegroundColor Red
        return $false
    }
}

# Verify all project paths exist
Write-Host "🔍 Checking project directories..." -ForegroundColor Yellow
$authExists = Test-ProjectPath $authPath "Authentication Service"
$apiExists = Test-ProjectPath $apiPath "API Service"
$uiExists = Test-ProjectPath $uiPath "UI Service"

if (-not ($authExists -and $apiExists -and $uiExists)) {
    Write-Host "❌ One or more project directories not found!" -ForegroundColor Red
    Write-Host "Make sure you're running this script from the OrderProcessingSystem root directory." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "🏁 Starting services in 3 seconds..." -ForegroundColor Green
Start-Sleep -Seconds 3

# Start Authentication Service
Write-Host "🔐 Starting Authentication Service..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command", 
    "cd '$authPath'; Write-Host '🔐 Authentication Service Starting...' -ForegroundColor Cyan; dotnet run"
)

# Wait a moment between starts
Start-Sleep -Seconds 2

# Start API Service  
Write-Host "🌐 Starting API Service..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$apiPath'; Write-Host '🌐 API Service Starting...' -ForegroundColor Cyan; dotnet run"
)

# Wait a moment between starts
Start-Sleep -Seconds 2

# Start UI Service
Write-Host "🎨 Starting UI Service..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", 
    "-Command",
    "cd '$uiPath'; Write-Host '🎨 UI Service Starting...' -ForegroundColor Cyan; dotnet run"
)

Write-Host ""
Write-Host "🎉 All services are starting!" -ForegroundColor Green
Write-Host "Check the opened PowerShell windows for each service status." -ForegroundColor Yellow
Write-Host ""
Write-Host "📋 Service URLs:" -ForegroundColor White
Write-Host "  🔐 Authentication: http://localhost:5270" -ForegroundColor Cyan
Write-Host "  🌐 API:            http://localhost:5000" -ForegroundColor Cyan
Write-Host "  🎨 UI:             http://localhost:5253" -ForegroundColor Cyan
Write-Host ""
Write-Host "⏳ Services may take 10-30 seconds to fully start." -ForegroundColor Yellow
Write-Host "💡 Close individual PowerShell windows to stop each service." -ForegroundColor Gray

# Wait for user input before closing this window
Write-Host ""
Read-Host "Press Enter to close this launcher window"
